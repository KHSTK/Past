using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Analytics.Internal;
using UnityEngine;
using UnityEngine.UI;

public class EventController : MonoBehaviour
{
    [Header("广播")]
    [SerializeField] private ObjectEventSO EventRoomOverEvent;
    [Header("引用")]
    [SerializeField] private UIDOMove uiDOMove;
    [SerializeField] private CurrentRoomStateSO currentRoomStateSO;
    [SerializeField] private PlayerStateSO playerStateSO;
    [SerializeField] private GameObject EventCanvas;
    [Header("UI")]
    [SerializeField] private Button Text;
    [SerializeField] private TextMeshProUGUI EventName;
    [SerializeField] private TextMeshProUGUI EventDescription;
    [SerializeField] private GameObject options;
    [SerializeField] private GameObject EventButton;
    public List<GameObject> EventIcons;
    private RoomEventData currentEventData;
    private string endDescription;
    private List<GameObject> optionButtons = new List<GameObject>();
    public void OnEnable()
    {
        foreach (var icon in EventIcons)
        {
            icon.SetActive(false);
        }
        SetText();
        Text.onClick.AddListener(OnTextClick);
        CreateOptionsButton();
    }

    private void OnTextClick()
    {
        //_ = uiDOMove.LeftMove(Text.GetComponent<RectTransform>(), 200, 0.5f);
        _ = uiDOMove.LeftMove(options.GetComponent<RectTransform>(), 400, 0.5f);
        //注销点击事件
        Text.onClick.RemoveAllListeners();
    }
    public void SetText()
    {
        currentEventData = currentRoomStateSO.currentRoomData.eventData;
        EventName.text = currentEventData.eventName;
        EventDescription.text = currentEventData.startDescription;
        EventButton.SetActive(true);
        if (currentEventData.eventID == "event_rhyme")
        {
            EventIcons[0].SetActive(true);
        }
        else if (currentEventData.eventID == "event_potion")
        {
            AudioManager.Instance.PlaySFX("Potion");
            EventIcons[3].SetActive(true);
        }
    }
    public void CreateOptionsButton()
    {
        foreach (var option in currentEventData.options)
        {
            var button = Instantiate(EventButton, options.transform);
            button.transform.Find("OptionName").GetComponent<TextMeshProUGUI>().text = option.optionName;
            button.transform.Find("OptionDescription").GetComponent<TextMeshProUGUI>().text = GetOptionDescription(option.effects);
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                ApplyOptionsEffect(option.effects);
                endDescription = option.endDescription;
                EndEvent();
            });
            optionButtons.Add(button);
        }
    }
    public void ApplyOptionsEffect(List<EventEffect> eventEffects)
    {
        foreach (var effect in eventEffects)
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
    public void EndEvent()
    {
        //_ = uiDOMove.RightMove(Text.GetComponent<RectTransform>(), 200, 0.5f);
        _ = uiDOMove.RightMove(options.GetComponent<RectTransform>(), 400, 0.5f);
        EventDescription.text = endDescription;
        foreach (var button in optionButtons)
        {
            Destroy(button);
        }
        Text.onClick.AddListener(OverEvent);
    }
    public void OverEvent()
    {
        Text.onClick.RemoveAllListeners();
        EventCanvas.SetActive(false);
        EventRoomOverEvent.RaiseEvent(this, this);
    }
    public void ChangeIcon(int iconIndex)
    {
        foreach (var icon in EventIcons)
        {
            icon.SetActive(false);
        }
        EventIcons[iconIndex].SetActive(true);
    }
}