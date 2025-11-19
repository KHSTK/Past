using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public static class ExcelToSOImporter
{
    [MenuItem("Tools/Import Enemy Data from CSV")]
    public static void ImportEnemyData()
    {
        // 1. 获取或创建 SO 实例
        EnemyConfigSO config = ScriptableObjectUtility.CreateOrGetAsset<EnemyConfigSO>(
            "Assets/GameDataSO/Config/EnemyConfig.asset");

        // 2. 读取 CSV 文件
        TextAsset csv = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Data/enemy_data.csv");
        if (csv == null)
        {
            Debug.LogError("CSV file not found!");
            return;
        }

        // 3. 清空旧数据（可选）
        config.enemyDataList.Clear();

        // 4. 解析 CSV 数据
        string[] rows = csv.text.Split('\n');
        for (int i = 1; i < rows.Length; i++) // 跳过标题行
        {
            string[] columns = rows[i].Split(',');

            // 5. 数据校验
            if (columns.Length < 7) continue;

            // 6. 创建 EnemyData 实例
            EnemyData data = new EnemyData
            {
                enemyID = columns[0],
                enemyName = columns[1],
                enemyDescription = columns[2],
                maxHp = int.Parse(columns[3]),
                damage = int.Parse(columns[4]),
                time = int.Parse(columns[5]),
                enemyType = (EnemyType)System.Enum.Parse(typeof(EnemyType), columns[6])
            };

            config.enemyDataList.Add(data);
        }

        // 7. 保存修改
        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
        Debug.Log($"Successfully imported {config.enemyDataList.Count} enemies!");
    }
}

// 辅助类：创建或获取 ScriptableObject
public static class ScriptableObjectUtility
{
    public static T CreateOrGetAsset<T>(string path) where T : ScriptableObject
    {
        T asset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
        }
        return asset;
    }
}