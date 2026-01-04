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

### 1. Pull the Latest Emulator Image

Update your local Docker cache:

```bash
docker pull mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator
```

This ensures you have the newest, non-expired version.

---

### 2. Remove the Expired Local Image

Docker may continue using the old image unless you remove it:

```bash
docker images | grep cosmos
docker rmi <image-id>
```

Replace `<image-id>` with the ID of the expired image.

---

## Verifying the Fix

After updating:

1. Stop your Aspire app
2. Run:

```bash
docker ps -a
```

3. Ensure no old emulator containers are still running
4. Restart your Aspire application — the emulator should now start normally

---

## Notes

- This issue will recur periodically unless you update the emulator image every few months.
- Using the `latest` tag reduces the chance of hitting expiration, but you may still need to pull updates occasionally.
- This expiration behavior is specific to the **Linux Cosmos DB Emulator**.
