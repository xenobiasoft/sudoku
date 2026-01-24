# Updating the Azure Cosmos DB Emulator for Aspire

## Fixing the “evaluation period has expired” error

When running an Aspire application that uses the Azure Cosmos DB Emulator, you may encounter this startup error:

```
Error: The evaluation period has expired.
./cosmosdb-emulator: ERROR: PAL initialization failed. Error: 104
```

This does **not** refer to your own evaluation period. It means the **Cosmos DB Emulator container image itself has expired**.

Microsoft ships the Linux emulator as a **time-limited evaluation build**, and older images stop working after a few months. When the expiration date passes, the emulator fails with the error above.

---

## Why This Happens

- The Cosmos DB Emulator for Linux is released as a temporary evaluation image.
- Aspire or Docker may be using a cached or pinned version that has since expired.
- When the expiration date passes, the emulator fails with the error above.

---

## How to Fix the Issue

### 1. Remove the emulator container

```bash
docker ps -a --filter "name=cosmosdb" --format "table {{.ID}}\t{{.Image}}\t{{.Names}}"
docker rm -f <container_id>
```

Replace `<container-id>` with the ID of the expired image.

---

## 2. Remove the cached emulator image

(you may have one of these, remove whichever matches your setup)

```bash
docker images | grep -i cosmos

docker rmi -f mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest
# or (if you see it)
docker rmi -f mcr.microsoft.com/cosmosdb/emulator:latest
```

---

### 3. Pull the Latest Emulator Image

Update your local Docker cache:

```bash
docker pull mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest
```

This ensures you have the newest, non-expired version.

---

## Notes

- This issue will recur periodically unless you update the emulator image every few months.
- Using the `latest` tag reduces the chance of hitting expiration, but you may still need to pull updates occasionally.
- This expiration behavior is specific to the **Linux Cosmos DB Emulator**.
