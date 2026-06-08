using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Lookloop.ItemManager
{
public partial class Core
{
    //用于增加一个float的时间time，用于辅助计时，所以特地装了一个结构体。
    struct HandleTime
    {
        public AsyncOperationHandle<ItemTable> handle;
        public float time;
    }
    //里面装着没有过期的句柄，作为缓存句柄
    readonly Dictionary<string, HandleTime> handleTimes = new();

    //获取itemtable的主要方法，也是这个addres的核心。
    public async Task<ItemTable> GetItemTable(string key)
    {
        //先检查一下是否有在没有过期的时间集合里面
        if (handleTimes.TryGetValue(key, out var entry))
        {
            if (entry.handle.IsValid() && entry.handle.Status == AsyncOperationStatus.Succeeded)
            {
                entry.time = Time.time;
                handleTimes[key] = entry;
                return entry.handle.Result;
            }

            handleTimes.Remove(key);
        }
        var handle = Addressables.LoadAssetAsync<ItemTable>(key);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            handleTimes[key] = new HandleTime { handle = handle, time = Time.time };
            return handle.Result;
        }

        Debug.LogError($"[Core] ItemTable 加载失败: {key}");
        Addressables.Release(handle);
        return null;
    }
    public void ReleaseStaleHandles()
    {
        var stale = new List<string>();

        foreach (var kv in handleTimes)
        {
            if (Time.time - kv.Value.time > 1800f)
                stale.Add(kv.Key);
        }

        foreach (var key in stale)
        {
            if (handleTimes.TryGetValue(key, out var entry) && entry.handle.IsValid())
                Addressables.Release(entry.handle);

            handleTimes.Remove(key);
            Debug.Log($"[Core] 卸载过期句柄: {key}");
        }
    }
}
}
