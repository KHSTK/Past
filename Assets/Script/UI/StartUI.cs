using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

[System.Serializable]
public class DialogueEntry
{
    public GameObject icon;          // 对应的图片
    public List<string> dialogues;   // 该图片对应的多句台词
    public List<string> dialoguseEnglish;   // 对应的英文台词   
}

public class StartUI : MonoBehaviour
{
    [Header("广播")]
    public ObjectEventSO StartUIOverEvent;
    [Header("UI Components")]
    public List<DialogueEntry> dialogueEntries = new List<DialogueEntry>();
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI dialogueTextEnglish;
    public Image canvasGroup; // 用于淡出效果

    [Header("Animation Settings")]
    public float fadeDuration = 0.5f;
    public float charPerSecond = 30f; // 打字速度
    public float dialogueInterval = 1f; // 台词间隔
    public float iconSwitchInterval = 1.5f; // 图片切换间隔

    private int currentEntryIndex = 0;
    private int currentDialogueIndex = 0;
    private bool isTyping = false;
    private Sequence typeSequence;
    private Coroutine autoPlayCoroutine;

    void OnEnable()
    {
        // 重置所有图标状态以确保一致
        foreach (var entry in dialogueEntries)
        {
            if (entry.icon != null)
            {
                entry.icon.SetActive(false);
                // 重置透明度
                CanvasGroup cg = GetOrAddCanvasGroup(entry.icon);
                cg.alpha = 1f;
            }
        }

        // 显示第一张图片
        if (dialogueEntries.Count > 0 && dialogueEntries[0].icon != null)
        {
            dialogueEntries[0].icon.SetActive(true);
        }

        // 重置UI背景
        canvasGroup.color = new Color(canvasGroup.color.r, canvasGroup.color.g, canvasGroup.color.b, 1f);

        // 重置所有变量
        currentEntryIndex = 0;
        currentDialogueIndex = 0;
        isTyping = false;
        dialogueText.text = "";
        dialogueTextEnglish.text = "";

        // 开始完全自动的对话序列
        autoPlayCoroutine = StartCoroutine(AutoPlayDialogueSequence());
    }

    void OnDisable()
    {
        // 确保停止所有协程
        if (autoPlayCoroutine != null)
        {
            StopCoroutine(autoPlayCoroutine);
            autoPlayCoroutine = null;
        }

        // 停止所有DOTween动画
        typeSequence?.Kill();
        typeSequence = null;

        // 重置透明度
        canvasGroup.color = new Color(canvasGroup.color.r, canvasGroup.color.g, canvasGroup.color.b, 1f);
    }

    IEnumerator AutoPlayDialogueSequence()
    {
        yield return null;

        while (currentEntryIndex < dialogueEntries.Count)
        {
            var currentEntry = dialogueEntries[currentEntryIndex];

            for (currentDialogueIndex = 0; currentDialogueIndex < currentEntry.dialogues.Count; currentDialogueIndex++)
            {
                string dialogue = currentEntry.dialogues[currentDialogueIndex];
                string dialogueEnglish = currentEntry.dialoguseEnglish[currentDialogueIndex];
                TypeText(dialogue, dialogueEnglish);

                while (isTyping)
                    yield return null;

                yield return new WaitForSeconds(dialogueInterval);
            }

            int currentIconIndex = currentEntryIndex;
            currentEntryIndex++;

            if (currentIconIndex < dialogueEntries.Count && currentEntryIndex < dialogueEntries.Count)
            {
                if (dialogueEntries[currentIconIndex].icon != null)
                {
                    FadeOutIcon(dialogueEntries[currentIconIndex].icon);
                }

                yield return new WaitForSeconds(iconSwitchInterval);

                if (dialogueEntries[currentEntryIndex].icon != null)
                {
                    FadeInIcon(dialogueEntries[currentEntryIndex].icon);
                }
            }
        }

        // 单独淡出所有图标
        foreach (var entry in dialogueEntries)
        {
            if (entry.icon != null && entry.icon.activeSelf)
            {
                FadeOutIcon(entry.icon);
            }
        }

        yield return new WaitForSeconds(fadeDuration);

        // 最后淡出背景
        FadeOutBackground();
    }

    void TypeText(string text, string textEnglish)
    {
        typeSequence?.Kill();

        isTyping = true;
        dialogueText.text = "";
        dialogueTextEnglish.text = "";

        float duration = text.Length / charPerSecond;

        typeSequence = DOTween.Sequence()
            .Append(
                DOTween.To(
                    () => dialogueText.text,
                    x => dialogueText.text = x,
                    text,
                    duration
                ).SetEase(Ease.Linear)
            )
            .Join(
                DOTween.To(
                    () => dialogueTextEnglish.text,
                    x => dialogueTextEnglish.text = x,
                    textEnglish,
                    duration
                ).SetEase(Ease.Linear)
            )
            .OnComplete(() => isTyping = false);
    }

    void FadeInIcon(GameObject icon)
    {
        if (icon == null) return;

        icon.SetActive(true);
        CanvasGroup cg = GetOrAddCanvasGroup(icon);
        cg.alpha = 0;
        cg.DOFade(1, fadeDuration);
    }

    void FadeOutIcon(GameObject icon)
    {
        if (icon == null) return;

        CanvasGroup cg = GetOrAddCanvasGroup(icon);
        cg.DOFade(0, fadeDuration)
            .OnComplete(() => icon.SetActive(false));
    }

    CanvasGroup GetOrAddCanvasGroup(GameObject obj)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null) cg = obj.AddComponent<CanvasGroup>();
        return cg;
    }

    public void FadeOutUI()
    {
        // 停止所有协程
        if (autoPlayCoroutine != null)
        {
            StopCoroutine(autoPlayCoroutine);
            autoPlayCoroutine = null;
        }

        // 停止所有DOTween动画
        typeSequence?.Kill();
        typeSequence = null;

        // 直接完成所有图标的淡出
        foreach (var entry in dialogueEntries)
        {
            if (entry.icon != null && entry.icon.activeSelf)
            {
                CanvasGroup cg = GetOrAddCanvasGroup(entry.icon);
                cg.alpha = 0;
                entry.icon.SetActive(false);
            }
        }

        // 直接完成背景淡出
        canvasGroup.color = new Color(canvasGroup.color.r, canvasGroup.color.g, canvasGroup.color.b, 0);
        gameObject.SetActive(false);
        StartUIOverEvent.RaiseEvent(this, this);
    }

    void FadeOutBackground()
    {
        // 只淡出背景
        canvasGroup.DOFade(0, fadeDuration)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                StartUIOverEvent.RaiseEvent(this, this);
            });
    }
}