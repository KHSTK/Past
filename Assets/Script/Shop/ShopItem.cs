using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [Header("UI")]
    private Image relicIcon;
    private Button selectButton;
    private TextMeshProUGUI priceText;
    [Header("广播")]
    public ObjectEventSO selectRelicEvent;
    private BaseRelic relicEffect;
    private RelicData currentRelicData;
    private int price;
    void Awake()
    {
        relicIcon = GetComponent<Image>();
        selectButton = GetComponent<Button>();
        priceText = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void Init(RelicData relicData)
    {
        currentRelicData = relicData;
        relicEffect = currentRelicData.relicEffect;
        price = currentRelicData.price;
        relicIcon.sprite = currentRelicData.relicIcon;
        priceText.text = price.ToString();
        selectButton.onClick.AddListener(SelectRelic);
        Debug.Log("初始化商店物品");
        Debug.Log("物品名称：" + currentRelicData.relicName);
    }
    private void SelectRelic()
    {
        selectRelicEvent.RaiseEvent(this, this);
    }
    public void UpdateAffordability(bool canAfford)
    {
        priceText.color = canAfford ? Color.white : Color.red;
    }
    public int GetPrice()
    {
        return price;
    }
    public RelicData GetRelicData()
    {
        return currentRelicData;
    }
}
