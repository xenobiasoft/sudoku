#!/usr/bin/env python3
"""
One-off data copy for the Cosmos DB free-tier -> paid-tier migration.
See docs/runbooks/cosmos-db-tier-migration.md for the full runbook.

Copies all items (or, with --since, only items changed at or after a given Unix
epoch timestamp) from every container in a source Cosmos DB database to a
same-named container in a destination database.

Intended usage — a single full copy with writes stopped (Phase 3 of the runbook):

    python migrate-cosmos-data.py \
      --src-endpoint https://cosmos-sudoku-prod.documents.azure.com:443/ \
      --src-key <old-account-primary-key> \
      --dst-endpoint https://cosmos-sudoku-prod2.documents.azure.com:443/ \
      --dst-key <new-account-primary-key> \
      --database sudoku --containers games profiles --prune

--prune deletes destination documents that no longer exist in the source, making
the copy idempotent: re-running converges the destination on the source instead
of accumulating orphans. This matters because the application hard-deletes games
and profiles without leaving a tombstone, so a copy alone can never remove
anything — a document deleted from the source after an earlier copy would
otherwise survive on the destination forever. Only run --prune once writes are
stopped; against a live source it races traffic and will delete documents the
destination should keep.

--since restricts the copy to items with _ts >= the given epoch, for an
incremental pass against a large source. The single-copy flow above does not
need it. Note that --since alone cannot observe deletions; pair it with --prune,
which always enumerates the full source regardless of --since.

Requires: pip install "azure-cosmos>=4.5,<5"

Keys are read from the CLI args or the SRC_KEY / DST_KEY env vars — never
commit real keys. Fetch them with:
  az cosmosdb keys list --name <account> -g <resource-group> --query primaryMasterKey -o tsv

This is a throwaway operational script, not part of the application build —
it is not referenced by any .csproj and has no test coverage requirement.
"""
import argparse
import os
import sys
import time

from azure.cosmos import CosmosClient

# Cosmos regenerates these on write; they carry no meaning in the destination.
SYSTEM_PROPERTIES = ("_rid", "_self", "_etag", "_attachments", "_ts")


def parse_args():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--src-endpoint", required=True, help="Source Cosmos DB account endpoint URL")
    parser.add_argument("--dst-endpoint", required=True, help="Destination Cosmos DB account endpoint URL")
    parser.add_argument("--src-key", default=os.environ.get("SRC_KEY"), help="Source account primary key (or set SRC_KEY env var)")
    parser.add_argument("--dst-key", default=os.environ.get("DST_KEY"), help="Destination account primary key (or set DST_KEY env var)")
    parser.add_argument("--database", default="sudoku", help="Database name (default: sudoku)")
    parser.add_argument("--containers", nargs="+", default=["games", "profiles"], help="Container names to copy (default: games profiles)")
    parser.add_argument("--since", type=int, default=None, help="Only copy items with _ts >= this Unix epoch (for delta sync)")
    parser.add_argument("--prune", action="store_true", help="Delete destination items that no longer exist in the source. Requires writes to be stopped.")
    parser.add_argument("--dry-run", action="store_true", help="Read and report counts without writing to or deleting from the destination")
    return parser.parse_args()


def partition_key_field(container):
    """The document property a container partitions on, e.g. 'gameId'."""
    paths = container.read()["partitionKey"]["paths"]
    return paths[0].lstrip("/")


def strip_system_properties(item):
    return {key: value for key, value in item.items() if key not in SYSTEM_PROPERTIES}


def copy_items(src_container, dst_container, since, dry_run):
    if since is None:
        items = src_container.read_all_items()
    else:
        # `>=` not `>`: _ts is second-granular, so a document written during the
        # same second the bulk copy started — but after that partition was already
        # scanned — would otherwise be missed by both passes. Upserts are idempotent,
        # so the one-second overlap costs nothing.
        items = src_container.query_items(
            query="SELECT * FROM c WHERE c._ts >= @since",
            parameters=[{"name": "@since", "value": since}],
        )

    count = 0
    for item in items:
        if not dry_run:
            dst_container.upsert_item(strip_system_properties(item))
        count += 1

    return count


def item_keys(container, pk_field):
    """Every (id, partition-key value) pair in a container."""
    query = f'SELECT c.id, c["{pk_field}"] AS pk FROM c'
    return {(row["id"], row.get("pk")) for row in container.query_items(query=query)}


def prune_items(src_container, dst_container, pk_field, dry_run):
    orphans = item_keys(dst_container, pk_field) - item_keys(src_container, pk_field)

    pruned = 0
    for item_id, partition_key in sorted(orphans, key=lambda pair: pair[0]):
        if partition_key is None:
            print(f"  SKIP {item_id}: no '{pk_field}' value to partition on", file=sys.stderr)
            continue
        if dry_run:
            print(f"  would delete {item_id} (partition {partition_key})")
        else:
            dst_container.delete_item(item=item_id, partition_key=partition_key)
        pruned += 1

    return pruned


def main():
    args = parse_args()

    if not args.src_key or not args.dst_key:
        print("ERROR: --src-key/--dst-key (or SRC_KEY/DST_KEY env vars) are required.", file=sys.stderr)
        sys.exit(1)

    start_epoch = int(time.time())
    print(f"Migration run started at epoch {start_epoch} ({time.ctime(start_epoch)}).")
    if args.prune and not args.dry_run:
        print("WARNING: --prune deletes destination documents that are absent from the source.")
        print("         Writes to the source must already be stopped (runbook Phase 3.1).")

    src_db = CosmosClient(args.src_endpoint, args.src_key).get_database_client(args.database)
    dst_db = CosmosClient(args.dst_endpoint, args.dst_key).get_database_client(args.database)

    total_copied = 0
    total_pruned = 0
    for container_name in args.containers:
        src_container = src_db.get_container_client(container_name)
        dst_container = dst_db.get_container_client(container_name)

        copied = copy_items(src_container, dst_container, args.since, args.dry_run)
        print(f"{container_name}: {'would copy' if args.dry_run else 'copied'} {copied} item(s)")
        total_copied += copied

        if args.prune:
            src_pk = partition_key_field(src_container)
            dst_pk = partition_key_field(dst_container)
            if src_pk != dst_pk:
                print(f"ERROR: {container_name} partition key differs (source '{src_pk}', destination '{dst_pk}').", file=sys.stderr)
                sys.exit(1)

            pruned = prune_items(src_container, dst_container, dst_pk, args.dry_run)
            print(f"{container_name}: {'would delete' if args.dry_run else 'deleted'} {pruned} orphaned item(s)")
            total_pruned += pruned

    print(f"Done. Total items {'that would be copied' if args.dry_run else 'copied'}: {total_copied}")
    if args.prune:
        print(f"      Total orphans {'that would be deleted' if args.dry_run else 'deleted'}: {total_pruned}")


if __name__ == "__main__":
    main()
