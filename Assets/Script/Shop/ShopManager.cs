using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public GameObject ShopItemPrefab;
    public RelicConfigSO relicConfig;
    private List<RelicData> currentrelics = new();
    private int refreshCost;
    private RelicData selectedRelic;
    private int selectedPrice;
    [SerializeField] private List<GameObject> selectedBars;
    [SerializeField] private PlayerStateSO playerStateSO;
    [SerializeField] private RelicManager relicManager;
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI playerCoinText;
    [SerializeField] private TextMeshProUGUI refreshCostText;
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private TextMeshProUGUI descriptionTitle;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [Header("广播")]
    public ObjectEventSO ShopOverEvent;
    private List<ShopItem> currentShopItems = new();
    private void Start()
    {
        refreshButton.onClick.AddListener(RefreshShop);
        buyButton.onClick.AddListener(TryBuyRelic);
    }
    private void OnEnable()
    {
        refreshCost = 1;
        SetCurrentRelics();
        SetItem();
        UpdateUI();
    }
    /// <summary>
    /// 初始化商店物品
    /// </summary>
    public void SetItem()
    {
        foreach (var item in currentShopItems)
        {
            //如果当前商店物品不足3个，则重新初始化
            if (currentrelics.Count < 3) SetCurrentRelics();
            Destroy(item.gameObject);
        }
        currentShopItems.Clear();
        for (int i = 0; i < 3; i++)
        {
            var item = Instantiate(ShopItemPrefab, transform);
            item.GetComponent<ShopItem>().Init(GetRandomItem());
            currentShopItems.Add(item.GetComponent<ShopItem>());
        }
        SelectItem(currentShopItems[0]);
        Debug.Log("当前数量" + currentrelics.Count);
    }
    /// <summary>
    /// 初始化当前商店可刷新物品
    /// </summary>
    public void SetCurrentRelics()
    {
        currentrelics.Clear();
        foreach (var relic in relicConfig.relics)
        {
            if (relicManager.activeRelics.Contains(relic.relicEffect)) continue;
            currentrelics.Add(relic);
        }
        //DOTO:将玩家已有的物品从当前商店可刷新物品中移除
    }
    /// <summary>
    /// 获取随机商品并从库存中移除
    /// </summary>
    /// <returns></returns>
    public RelicData GetRandomItem()
    {
        var randomItem = currentrelics[Random.Range(0, currentrelics.Count)];
        currentrelics.Remove(randomItem);
        return randomItem;
    }
    /// <summary>
    /// 刷新商店
    /// </summary>
    public void RefreshShop()
    {
        SetItem();
        playerStateSO.AddCoin(-refreshCost);
        refreshCost++;
        UpdateUI();
    }
    public void SelectItem(object item)
    {
        ShopItem shopItem = item as ShopItem;
        UnselectItemBG(currentShopItems.IndexOf(shopItem));
        selectedRelic = shopItem.GetRelicData();
        descriptionPanel.SetActive(true);
        descriptionTitle.text = selectedRelic.relicName;
        descriptionText.text = selectedRelic.relicDescription;
        selectedPrice = shopItem.GetPrice();
        UpdateBuyButtonState();
    }
    private void TryBuyRelic()
    {
        if (selectedRelic == null) return;

        if (playerStateSO.currentCoin >= selectedPrice)
        {
            playerStateSO.currentCoin -= selectedPrice;
            relicManager.AddRelic(selectedRelic.relicEffect);
            RemovePurchasedItem();
            selectedRelic = null;
            descriptionPanel.SetActive(false);
            UpdateUI();
        }
    }

    private void RemovePurchasedItem()
    {
        var itemToRemove = currentShopItems.Find(item =>
            item.GetComponent<ShopItem>().GetRelicData() == selectedRelic);
        if (itemToRemove != null)
        {
            currentShopItems.Remove(itemToRemove);
            Destroy(itemToRemove.gameObject);
        }
    }
    public void UnselectItemBG(int amount)
    {
        foreach (var item in selectedBars)
        {
            item.SetActive(false);
        }
        selectedBars[amount].SetActive(true);
    }
    /// <summary>
    /// 更新UI
    /// </summary>
    private void UpdateUI()
    {
        refreshButton.interactable = playerStateSO.currentCoin >= refreshCost;
        playerCoinText.text = playerStateSO.currentCoin.ToString();
        refreshCostText.text = refreshCost.ToString();
        foreach (var item in currentShopItems)
        {
            item.UpdateAffordability(playerStateSO.currentCoin >= item.GetPrice());
        }
        UpdateBuyButtonState();
    }
    private void UpdateBuyButtonState()
    {
        buyButton.interactable = selectedRelic != null && playerStateSO.currentCoin >= selectedPrice;
    }

}
