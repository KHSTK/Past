using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private MapSaveDataSO mapSaveDataSO;

    // 路径管理工具类
    public static class PathHelper
    {
        // 获取基础存档目录
        public static string SaveDirectory =>
            Path.Combine(Application.persistentDataPath, "Saves");

        // 获取具体存档路径
        public static string GetSavePath(string fileName)
        {
            // 自动创建目录（如果不存在）
            Directory.CreateDirectory(SaveDirectory);

            // 处理特殊字符并添加扩展名
            string sanitizedName = SanitizeFileName(fileName);
            return Path.Combine(SaveDirectory, $"{sanitizedName}.save");
        }

        // 文件名规范化（防止非法字符）
        private static string SanitizeFileName(string fileName)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            return string.Join("_", fileName.Split(invalidChars));
        }
    }
    public void SaveGame(string slotName = "autosave")
    {
        // 使用统一路径获取方法
        string savePath = PathHelper.GetSavePath(slotName);

        // 原子写入操作（防止写入中断导致数据损坏）
        string tempPath = $"{savePath}.tmp";

        // 构造存档数据
        var saveData = new MapSaveDataSO
        {
            seed = mapSaveDataSO.seed,
            rooms = new List<RoomSaveData>(mapSaveDataSO.rooms)
        };

        // 序列化并保存
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(tempPath, json);

        // 替换旧文件
        if (File.Exists(savePath)) File.Delete(savePath);
        File.Move(tempPath, savePath);
        Debug.Log($"存档成功: {savePath}");
    }

    public void LoadGame(string slotName = "autosave")
    {
        string savePath = PathHelper.GetSavePath(slotName);

        if (!File.Exists(savePath))
        {
            Debug.LogError($"存档不存在: {savePath}");
            return;
        }

        string json = File.ReadAllText(savePath);
        MapSaveDataSO saveData = JsonUtility.FromJson<MapSaveDataSO>(json);

        // 应用加载数据
        mapSaveDataSO.seed = saveData.seed;
        mapSaveDataSO.rooms = new List<RoomSaveData>(saveData.rooms);

        Debug.Log($"读档成功: {savePath}");
    }
}
