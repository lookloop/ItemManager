using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public partial class UIResponder
{
    /// <summary>
    /// 加载 ItemTable 资源 (Addressables async/await)
    /// </summary>
    public async Task<ItemTable> GetItemTable(string key)
    {
        AsyncOperationHandle<ItemTable> handle = Addressables.LoadAssetAsync<ItemTable>(key);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            return handle.Result;
        }

        Debug.LogError($"[UIResponder] ItemTable 加载失败: {key}");
        Addressables.Release(handle);
        return null;
    }
}
