using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/DialogueConfig")]
public class DialogueConfigSO : ScriptableObject
{
    public List<DialogueData> dialogues = new List<DialogueData>();

    public DialogueData GetDialogue(string dialogueID)
    {
        return dialogues.Find(d => d.dialogueID == dialogueID);
    }
}

// DialogueData.cs
[System.Serializable]
public class DialogueData
{
    public string dialogueID;
    public List<DialogueNode> nodes = new List<DialogueNode>();
    public string startNodeID; // 起始节点ID
}

[System.Serializable]
public class DialogueNode
{
    public string nodeID;
    public string speakerName; // 对话者名字
    [TextArea] public string dialogueText;
    public List<DialogueOption> options = new List<DialogueOption>();
    public string nextNodeID; // 默认下一个节点
    public bool showOptionsImmediately; // 是否立即显示选项
}

[System.Serializable]
public class DialogueOption
{
    public string optionName;
    public List<EventEffect> effects;
    [TextArea] public string responseText; // 选项响应文本
    public string nextNodeID; // 选择后的下一个节点
}
