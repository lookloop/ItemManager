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

    /// <summary>
    /// 加载 ItemTable 资源。
    /// 同一 key 只存一份句柄，命中缓存直接返回 ScriptableObject 引用，不触发 IO。
    /// 30 分钟未使用的句柄由 ReleaseStaleHandles 定时回收。
    /// </summary>
    public async Task<ItemTable> GetItemTable(string key)
    {
        // 命中 → 更新时间，拿同一个 ScriptableObject 引用
        if (_tableCache.TryGetValue(key, out var entry))
        {
            if (entry.handle.IsValid() && entry.handle.Status == AsyncOperationStatus.Succeeded)
            {
                entry.lastUsed = Time.time;
                _tableCache[key] = entry;
                return entry.handle.Result;
            }

            // 句柄已失效 → 清除，重新加载
            _tableCache.Remove(key);
        }

        // 未命中 → Addressables 加载
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

    /// <summary>回收超过 30 分钟未使用的句柄</summary>
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
