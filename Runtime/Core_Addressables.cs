using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Lookloop.ItemManager
{
public partial class Core
{
    struct CacheEntry
    {
        public AsyncOperationHandle<ItemTable> handle;
        public float lastUsed;
    }

    readonly Dictionary<string, CacheEntry> _tableCache = new();

 
    public async Task<ItemTable> GetItemTable(string key)
    {
        if (_tableCache.TryGetValue(key, out var entry))
        {
            if (entry.handle.IsValid() && entry.handle.Status == AsyncOperationStatus.Succeeded)
            {
                entry.lastUsed = Time.time;
                _tableCache[key] = entry;
                return entry.handle.Result;
            }

            _tableCache.Remove(key);
        }
        var handle = Addressables.LoadAssetAsync<ItemTable>(key);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _tableCache[key] = new CacheEntry { handle = handle, lastUsed = Time.time };
            return handle.Result;
        }

        Debug.LogError($"[Core] ItemTable 加载失败: {key}");
        Addressables.Release(handle);
        return null;
    }
    public void ReleaseStaleHandles()
    {
        var stale = new List<string>();

        foreach (var kv in _tableCache)
        {
            if (Time.time - kv.Value.lastUsed > 1800f)
                stale.Add(kv.Key);
        }

        foreach (var key in stale)
        {
            if (_tableCache.TryGetValue(key, out var entry) && entry.handle.IsValid())
                Addressables.Release(entry.handle);

            _tableCache.Remove(key);
            Debug.Log($"[Core] 卸载过期句柄: {key}");
        }
    }
}
}
