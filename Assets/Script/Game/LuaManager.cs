using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using XLua;

[System.Serializable]
public class LuaScriptReference
{
    public string scriptName;
    public AssetReferenceT<TextAsset> assetReference;
}

public class LuaManager : MonoBehaviour
{
    public static LuaManager Instance { get; private set; }

    [Header("Lua脚本配置")]
    public List<LuaScriptReference> luaScripts = new List<LuaScriptReference>();

    private LuaEnv luaEnv;
    private Dictionary<string, LuaTable> loadedScripts = new Dictionary<string, LuaTable>();
    public bool IsInitialized { get; private set; } = false;

    public event Action OnLuaInitialized;

    // Lua表属性，方便访问
    public LuaTable ComboConfig { get; private set; }

    private async void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            await InitializeLuaAsync();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async Task InitializeLuaAsync()
    {
        try
        {
            luaEnv = new LuaEnv();

            // 设置自定义加载器
            luaEnv.AddLoader(CustomLuaLoader);

            // 加载所有配置的Lua脚本
            foreach (var scriptRef in luaScripts)
            {
                await LoadLuaScriptAsync(scriptRef.scriptName, scriptRef.assetReference);
            }

            IsInitialized = true;
            OnLuaInitialized?.Invoke();
            Debug.Log("Lua管理器初始化完成");
        }
        catch (Exception e)
        {
            Debug.LogError($"Lua管理器初始化失败: {e.Message}");
        }
    }

    /// <summary>
    /// 加载指定路径的Lua文件
    /// </summary>
    /// <param name="filepath">引用类型的文件路径字符串，方法内部可能会修改此路径</param>
    private byte[] CustomLuaLoader(ref string filepath)
    {
        return null; // 临时返回null，实际实现中应返回加载的Lua文件内容
    }

    /// <summary>
    /// Addressable异步加载Lua脚本文件
    /// </summary>
    /// <param name="scriptName">Lua脚本名称</param>
    /// <param name="assetRef">Lua脚本资源的引用</param>
    private async Task LoadLuaScriptAsync(string scriptName, AssetReferenceT<TextAsset> assetRef)
    {
        // 检查资源引用是否为空
        if (assetRef == null)
        {
            Debug.LogError($"Lua资源引用为空: {scriptName}");
            return;
        }
        try
        {
            // 异步加载Lua脚本资源
            var handle = assetRef.LoadAssetAsync<TextAsset>();
            await handle.Task;

            // 检查异步操作是否成功完成
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // 获取加载的文本资源
                TextAsset luaText = handle.Result;

                // 执行Lua脚本内容，并将结果存储在result数组中
                object[] result = luaEnv.DoString(luaText.text, scriptName);

                // 获取全局表
                LuaTable scriptTable = luaEnv.Global.Get<LuaTable>(scriptName);

                if (scriptTable != null)
                {
                    loadedScripts[scriptName] = scriptTable;

                    // 特殊处理combo_config
                    if (scriptName == "ComboConfig")
                    {
                        ComboConfig = scriptTable;
                    }

                    Debug.Log($"成功加载Lua脚本: {scriptName}");
                }
                else
                {
                    Debug.LogWarning($"Lua脚本未返回表: {scriptName}");
                }

                // 释放资源句柄
                Addressables.Release(handle);
            }
            else
            {
                Debug.LogError($"加载Lua脚本失败: {scriptName}, 状态: {handle.Status}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"加载Lua脚本异常 {scriptName}: {e.Message}");
        }
    }

    public T GetValue<T>(string scriptName, string key)
    {
        if (!IsInitialized || !loadedScripts.ContainsKey(scriptName))
        {
            Debug.LogWarning($"Lua脚本未加载: {scriptName}");
            return default(T);
        }

        try
        {
            return loadedScripts[scriptName].Get<T>(key);
        }
        catch (Exception e)
        {
            Debug.LogError($"获取Lua值失败 {scriptName}.{key}: {e.Message}");
            return default(T);
        }
    }

    public LuaTable GetTable(string scriptName, string key)
    {
        if (!IsInitialized || !loadedScripts.ContainsKey(scriptName))
        {
            Debug.LogWarning($"Lua脚本未加载: {scriptName}");
            return null;
        }

        try
        {
            return loadedScripts[scriptName].Get<LuaTable>(key);
        }
        catch (Exception e)
        {
            Debug.LogError($"获取Lua表失败 {scriptName}.{key}: {e.Message}");
            return null;
        }
    }

    // 热更新方法
    public async Task HotUpdateScript(string scriptName, AssetReferenceT<TextAsset> newAssetRef)
    {
        if (loadedScripts.ContainsKey(scriptName))
        {
            loadedScripts.Remove(scriptName);
        }

        await LoadLuaScriptAsync(scriptName, newAssetRef);
        Debug.Log($"Lua脚本热更新完成: {scriptName}");
    }

    private void OnDestroy()
    {
        if (luaEnv != null)
        {
            luaEnv.Dispose();
        }
    }

    private void Update()
    {
        // XLua建议定期调用GC
        if (luaEnv != null)
        {
            luaEnv.Tick();
        }
    }
}