using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;


namespace Lookloop.ItemManager
{
public partial class Core
{
    

    /// <summary>
    /// Wraps an Addressables handle with a timestamp so the cache expiry
    /// loop can release stale entries.
    /// </summary>
    class HandleTime
    {
        public AsyncOperationHandle<ItemTable> handle;
        public float time;
    }
    IEnumerator LossTimeLoop()
    {
        yield return new WaitForSeconds(checkTime);
        while (true)
        {
            LossTime();
            yield return new WaitForSeconds(checkTime);
        }
    }
    // In‑memory cache: only unexpired handles live here
    readonly Dictionary<string, HandleTime> handleTimes = new();

    /// <summary>
    /// Load an <c>ItemTable</c> by its Addressables key.
    /// Hits the in‑memory cache first; falls back to an async load.
    /// </summary>
    public async Task<ItemTable> GetItemTable(string key)
    {
        // Cache hit — reuse the existing handle if it's still valid
        if (handleTimes.TryGetValue(key, out var entry))
        {
            // Confirm the handle is still alive and fully loaded
            if (entry.handle.IsValid() && entry.handle.Status == AsyncOperationStatus.Succeeded)
            {
                entry.time = Time.time;
                return entry.handle.Result;
            }
            // Stale or failed handle — evict from cache
            handleTimes.Remove(key);
        }
        // Cache miss — start a fresh async load
        var handle = Addressables.LoadAssetAsync<ItemTable>(key);
        // Await the async operation
        await handle.Task;
        // On success, store in the cache with a fresh timestamp
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            // Timestamp with current game time; handle is fresh from the loader
            handleTimes[key] = new HandleTime { handle = handle, time = Time.time };
            // Return the result — slower path than a cache hit
            return handle.Result;
        }
        // Release the failed handle (address has been resolved via await)
        Addressables.Release(handle);
        // Let Launch() catch this and display the error via tmpTip
        throw new System.Exception($"[Core] Failed to load ItemTable: {key}");
    }
    /// <summary>
    /// Iterate the cache and release any handle whose timestamp exceeds
    /// <c>retainTime</c>. Called periodically by <c>LossTimeLoop</c>.
    /// </summary>
    public void LossTime()
    {
        // Collect expired keys first to avoid mutating the dict during enumeration
        var loss = new List<string>();
        foreach (var a in handleTimes)
        {
            if (Time.time - a.Value.time > retainTime)
                loss.Add(a.Key);
        }
        foreach (var key in loss)
        {
            // Is the entry still in the dict and is the handle alive?
            if (handleTimes.TryGetValue(key, out var entry) && entry.handle.IsValid())
                // Release the Addressables asset first
                Addressables.Release(entry.handle);

            // Then remove from the cache dict
            handleTimes.Remove(key);
            Debug.Log($"[Core] Expired handle released: {key}");
        }
    }
}
}
