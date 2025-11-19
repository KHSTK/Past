// DialogueController.cs
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    [Header("广播")]
    [SerializeField] private StringEventSO OnDialogueEnd; // 使用字符串事件传递对话ID

    [Header("引用")]
    [SerializeField] private DialogueConfigSO dialogueConfig;
    [SerializeField] private PlayerStateSO playerStateSO;
    [SerializeField] private List<GameObject> RestIcon;

    [Header("UI")]
    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private TextMeshProUGUI dialogueName;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject optionButtonPrefab;
    [SerializeField] private Button continueButton;

    private DialogueData currentDialogue;
    private DialogueNode currentNode;
    private List<GameObject> currentOptionButtons = new List<GameObject>();

    // 外部调用开始对话
    public void StartDialogue(string dialogueID)
    {
        currentDialogue = dialogueConfig.GetDialogue(dialogueID);
        if (currentDialogue == null)
        {
            Debug.LogError($"Dialogue not found: {dialogueID}");
            return;
        }
        Debug.Log($"Starting dialogue: {dialogueID}");
        dialogueCanvas.SetActive(true);
        SetNode(currentDialogue.startNodeID);
    }

    private void SetNode(string nodeID)
    {
        currentNode = currentDialogue.nodes.Find(n => n.nodeID == nodeID);
        if (currentNode == null)
        {
            EndDialogue();
            return;
        }

        ClearOptions();
        dialogueName.text = currentNode.speakerName;
        dialogueText.text = currentNode.dialogueText;

        // 处理选项显示逻辑
        if (currentNode.showOptionsImmediately && currentNode.options.Count > 0)
        {
            ShowOptions();
        }
        else
        {
            ShowContinueButton();
        }
    }

    private void ShowContinueButton()
    {
        optionsPanel.SetActive(false);
        continueButton.gameObject.SetActive(true);

        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() =>
        {
            if (!string.IsNullOrEmpty(currentNode.nextNodeID))
            {
                SetNode(currentNode.nextNodeID);
            }
            else
            {
                EndDialogue();
            }
        });
    }

    private void ShowOptions()
    {
        continueButton.gameObject.SetActive(false);
        optionsPanel.SetActive(true);

        foreach (var option in currentNode.options)
        {
            GameObject buttonObj = Instantiate(optionButtonPrefab, optionsPanel.transform);
            buttonObj.transform.Find("OptionName").GetComponent<TextMeshProUGUI>().text = option.optionName;
            buttonObj.transform.Find("OptionDescription").GetComponent<TextMeshProUGUI>().text = GetOptionDescription(option.effects);

            Button button = buttonObj.GetComponent<Button>();
            button.onClick.AddListener(() => OnOptionSelected(option));

            currentOptionButtons.Add(buttonObj);
        }
    }

    private void OnOptionSelected(DialogueOption option)
    {
        // 应用效果
        ApplyEffects(option.effects);

        // 显示响应文本
        if (!string.IsNullOrEmpty(option.responseText))
        {
            dialogueText.text = option.responseText;
        }

        ClearOptions();
        continueButton.gameObject.SetActive(true);

        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() =>
        {
            if (!string.IsNullOrEmpty(option.nextNodeID))
            {
                SetNode(option.nextNodeID);
            }
            else
            {
                EndDialogue();
            }
        });
    }

    private void ApplyEffects(List<EventEffect> effects)
    {
        foreach (var effect in effects)
        {
            switch (effect.effectType)
            {
                case EffectType.AddCoin:
                    playerStateSO.AddCoin(effect.intValue);
                    break;
                case EffectType.AddCritical:
                    playerStateSO.AddCritical(effect.intValue);
                    break;
                case EffectType.ModifyPlayerHP:
                    playerStateSO.AddHp(effect.intValue);
                    break;
                case EffectType.ChangeIcon:
                    ChangeIcon(effect.intValue);
                    break;
            }
        }
    }
    public string GetOptionDescription(List<EventEffect> eventEffects)
    {
        string optionDescription = "";
        foreach (var effect in eventEffects)
        {
            switch (effect.effectType)
            {
                case EffectType.AddCoin:
                    optionDescription += "获得" + effect.intValue + "金币\n";
                    break;
                case EffectType.AddCritical:
                    optionDescription += "获得" + effect.intValue + "暴击率\n";
                    break;
                case EffectType.ModifyPlayerHP:
                    optionDescription += "获得" + effect.intValue + "生命值\n";
                    break;
            }
        }
        return optionDescription;
    }
    public void ChangeIcon(int iconID)
    {
        foreach (var icon in RestIcon)
        {
            icon.SetActive(false);
        }
        RestIcon[iconID].SetActive(true);
    }
    private void ClearOptions()
    {
        foreach (var button in currentOptionButtons)
        {
            Destroy(button);
        }
        currentOptionButtons.Clear();
    }

    private void EndDialogue()
    {
        dialogueCanvas.SetActive(false);
        OnDialogueEnd.RaiseEvent(currentDialogue.dialogueID, this);
        Debug.Log($"Dialogue ended: {currentDialogue.dialogueID}");
        ChangeIcon(0);
    }
}