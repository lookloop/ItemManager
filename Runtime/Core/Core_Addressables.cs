using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Lookloop.ItemManager
{
public partial class Core
{

    //用于增加一个float的时间time，用于辅助计时。
    class HandleTime
    {
        public AsyncOperationHandle<ItemTable> handle;
        public float time;
    }
    //里面装着没有过期的句柄，作为缓存句柄
    readonly Dictionary<string, HandleTime> handleTimes = new();

    //获取itemtable的主要方法，也是这个addres的核心。
    public async Task<ItemTable> GetItemTable(string key)
    {
        //通过key取结构体，var=HandleTime，如果取成功了就用缓存的。
        if (handleTimes.TryGetValue(key, out var entry))
        {
            //先问问，这个key在实际addres还有实际的东西么
            //再问问，是否加载成功。加载中和加载失败就不行。
            if (entry.handle.IsValid() && entry.handle.Status == AsyncOperationStatus.Succeeded)
            {
                entry.time = Time.time;
                return entry.handle.Result;
            }
            //加载没有成功，if没有成功，所以没有return。那么就代表这个key，用不了。先清理了。
            handleTimes.Remove(key);
        }
        //if没有成功，用缓存没有成功。开始自己加载。
        //声明加载。
        var handle = Addressables.LoadAssetAsync<ItemTable>(key);
        //异步加载，先去做别的，加载成功了再通知。
        await handle.Task;
        //判断加载情况，如果成功，那就存入缓存。
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            //时间刷新为最新游戏运行时间，句柄也是刚刚出炉。
            handleTimes[key] = new HandleTime { handle = handle, time = Time.time };
            //返回handle给外部，没有命中缓存有点慢。
            return handle.Result;
        }
        //加载失败，提醒一下正在编辑器的开发者。
        Debug.LogError($"[Core] ItemTable 加载失败: {key}");
        //释放这个有问题的句柄，因为await了，句柄有了，同时又加载失败，释放一下把。
        Addressables.Release(handle);
        //没东西可以返回，直接null，外部记住防null。
        return null;
    }
    public void LossTime()
    {
        // 直接遍历字典，同步释放过期资源 + 移除条目
        // 先收集过期 key，避免在遍历中修改字典
        var loss = new List<string>();
        foreach (var a in handleTimes)
        {
            if (Time.time - a.Value.time > 1800f)
                loss.Add(a.Key);
        }
        foreach (var key in loss)
        {
            //字典还有吗？addres句柄指向的资源还有吗？
            if (handleTimes.TryGetValue(key, out var entry) && entry.handle.IsValid())
                //先释放实际资源。
                Addressables.Release(entry.handle);

            //资源释放完了，开始释放字典。
            handleTimes.Remove(key);
            //双释放完毕。
            Debug.Log($"[Core] 卸载过期句柄: {key}");
        }
    }
}
}
