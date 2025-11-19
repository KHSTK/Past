#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class CSVImporter : EditorWindow
{
    private string eventsPath = "Assets/Data/Events.csv";
    private string eventOptionsPath = "Assets/Data/EventOptions.csv";
    private string dialoguesPath = "Assets/Data/Dialogues.csv";
    private string nodesPath = "Assets/Data/Nodes.csv";
    private string optionsPath = "Assets/Data/Options.csv";

    [MenuItem("Tools/Configs/Import from CSV")]
    public static void ShowWindow()
    {
        GetWindow<CSVImporter>("Config Importer");
    }

    void OnGUI()
    {
        // 1. 事件房间配置路径
        GUILayout.Label("Event Room Config Paths", EditorStyles.boldLabel);
        eventsPath = EditorGUILayout.TextField("Events CSV", eventsPath);
        eventOptionsPath = EditorGUILayout.TextField("Event Options CSV", eventOptionsPath);

        GUILayout.Space(15);

        // 2. 对话配置路径
        GUILayout.Label("Dialogue Config Paths", EditorStyles.boldLabel);
        dialoguesPath = EditorGUILayout.TextField("Dialogues CSV", dialoguesPath);
        nodesPath = EditorGUILayout.TextField("Nodes CSV", nodesPath);
        optionsPath = EditorGUILayout.TextField("Options CSV", optionsPath);

        GUILayout.Space(25);

        if (GUILayout.Button("Import All Configs"))
        {
            ImportEventRoomConfig();
            ImportDialogueConfig();
            AssetDatabase.Refresh();
            Debug.Log("All configs imported successfully!");
        }
    }

    // ===================== 事件房间配置导入 =====================
    private void ImportEventRoomConfig()
    {
        var eventsData = CSVUtility.ParseCSV(eventsPath);
        var eventOptionsData = CSVUtility.ParseCSV(eventOptionsPath);

        EventRoomConfigSO config = CreateOrLoadAsset<EventRoomConfigSO>("Assets/GameDataSO/Config/EventRoomConfig.asset");
        config.events = new List<RoomEventData>();

        foreach (var eventRow in eventsData)
        {
            RoomEventData eventData = new RoomEventData
            {
                eventID = GetCsvValue(eventRow, "eventID"),
                eventName = GetCsvValue(eventRow, "eventName"),
                startDescription = GetCsvValue(eventRow, "startDescription"),
                options = new List<EventOption>()
            };

            // 添加选项
            List<Dictionary<string, string>> options = eventOptionsData.FindAll(
                o => GetCsvValue(o, "eventID") == eventData.eventID
            );

            foreach (var optionRow in options)
            {
                EventOption option = new EventOption
                {
                    optionName = GetCsvValue(optionRow, "optionName"),
                    endDescription = GetCsvValue(optionRow, "endDescription"),
                    effects = ParseEffects(GetCsvValue(optionRow, "effects"))
                };

                eventData.options.Add(option);
            }

            config.events.Add(eventData);
        }

        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
        Debug.Log($"Imported {config.events.Count} events");
    }

    // ===================== 对话配置导入 =====================
    private void ImportDialogueConfig()
    {
        var dialoguesData = CSVUtility.ParseCSV(dialoguesPath);
        var nodesData = CSVUtility.ParseCSV(nodesPath);
        var optionsData = CSVUtility.ParseCSV(optionsPath);

        DialogueConfigSO config = CreateOrLoadAsset<DialogueConfigSO>("Assets/GameDataSO/Config/DialogueConfig.asset");
        config.dialogues = new List<DialogueData>();

        foreach (var dialogueRow in dialoguesData)
        {
            DialogueData dialogue = new DialogueData
            {
                dialogueID = GetCsvValue(dialogueRow, "dialogueID"),
                startNodeID = GetCsvValue(dialogueRow, "startNodeID"),
                nodes = new List<DialogueNode>()
            };

            // 添加节点
            List<Dictionary<string, string>> nodes = nodesData.FindAll(
                n => GetCsvValue(n, "dialogueID") == dialogue.dialogueID
            );

            foreach (var nodeRow in nodes)
            {
                DialogueNode node = new DialogueNode
                {
                    nodeID = GetCsvValue(nodeRow, "nodeID"),
                    speakerName = GetCsvValue(nodeRow, "speakerName"),
                    dialogueText = GetCsvValue(nodeRow, "dialogueText"),
                    nextNodeID = GetCsvValue(nodeRow, "nextNodeID"),
                    showOptionsImmediately = GetCsvValue(nodeRow, "showOptionsImmediately") == "TRUE",
                    options = new List<DialogueOption>()
                };

                // 添加选项
                List<Dictionary<string, string>> options = optionsData.FindAll(
                    o => GetCsvValue(o, "nodeID") == node.nodeID
                );

                foreach (var optionRow in options)
                {
                    DialogueOption option = new DialogueOption
                    {
                        optionName = GetCsvValue(optionRow, "optionName"),
                        responseText = GetCsvValue(optionRow, "responseText"),
                        nextNodeID = GetCsvValue(optionRow, "nextNodeID"),
                        effects = ParseEffects(GetCsvValue(optionRow, "effects"))
                    };

                    node.options.Add(option);
                }

                dialogue.nodes.Add(node);
            }

            config.dialogues.Add(dialogue);
        }

        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
        Debug.Log($"Imported {config.dialogues.Count} dialogues");
    }

    // ===================== 工具方法 =====================
    private T CreateOrLoadAsset<T>(string path) where T : ScriptableObject
    {
        T asset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (asset == null)
        {
            asset = CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
        }
        return asset;
    }

    private string GetCsvValue(Dictionary<string, string> row, string key)
    {
        return row.ContainsKey(key) ? row[key] : "";
    }

    private List<EventEffect> ParseEffects(string jsonString)
    {
        if (string.IsNullOrEmpty(jsonString))
            return new List<EventEffect>();

        try
        {
            return JsonUtility.FromJson<EffectWrapper>("{\"effects\":" + jsonString + "}").effects;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON解析错误: {e.Message}\n内容: {jsonString}");
            return new List<EventEffect>();
        }
    }

    [System.Serializable]
    private class EffectWrapper
    {
        public List<EventEffect> effects;
    }
}
#endif