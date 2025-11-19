using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class CardDeck : MonoBehaviour
{
    public PlayerStateSO playerState;
    [Header("广播")]
    public ObjectEventSO selectedCardEvent;
    public ObjectEventSO disSelectCardEvent;
    public IntEventSO discardNumberEvent;
    public CardManager cardManager;
    public CardLayoutManager cardLayoutManager;
    public SortingMode sortingMode;
    [Header("牌堆")]
    [SerializeField]
    private List<CardData> drawDeck = new();//抽牌堆
    private List<CardData> discardDeck = new();//弃牌堆
    private List<CardEntity> handleCardObjectList = new();//手牌
    public List<CardEntity> selectedCardList = new();//选中堆
    public Vector3 deckPos;//牌堆位置
    public int drawCount;//每回合抽牌数量
    public int maxCard;//手牌上限
    public int MaxDiscardNumber;//弃牌上限
    private int discardNumber;
    private void Start()
    {
        sortingMode = SortingMode.ByRank;
        maxCard = 8;
        MaxDiscardNumber = 2;
        Init();
    }
    public void Init()
    {
        drawDeck.Clear();
        foreach (CardData card in cardManager.allCardList)
        {
            drawDeck.Add(card);
        }
        ResetDiscardNumber();
    }
    [ContextMenu("抽牌")]
    public void DrawCard()
    {
        Debug.Log("测试种子" + Random.Range(20, 50));
        ShuffleDeck();
        DrawCard(maxCard);
    }
    //回收所有手牌，回到菜单时调用
    public void DiscardAllCard()
    {
        Debug.Log("手牌数：" + handleCardObjectList.Count);
        int amount = handleCardObjectList.Count;
        for (int i = 0; i < amount; i++)
        {
            DiscardCard(handleCardObjectList[0]);
        }
        foreach (var item in discardDeck)
        {
            //将弃牌堆中的牌放回抽牌堆
            drawDeck.Add(item);
        }
        //清空弃牌堆
        discardDeck.Clear();
        selectedCardList.Clear();
    }
    /// <summary>
    /// 抽牌
    /// </summary>
    /// <param name="aomunt">抽牌数量</param>
    public void DrawCard(int aomunt)
    {
        for (int i = 0; i < aomunt; i++)
        {
            //如果抽牌堆为空，则补满并洗牌
            if (drawDeck.Count == 0)
            {
                foreach (var item in discardDeck)
                {
                    //将弃牌堆中的牌放回抽牌堆
                    drawDeck.Add(item);
                }
                //清空弃牌堆
                discardDeck.Clear();
                ShuffleDeck();
            }
            CardData currentCardData = drawDeck[0];
            drawDeck.RemoveAt(0);
            //Debug.Log("抽到花色：" + currentCardData.suit + "牌号：" + currentCardData.rank + "伤害:" + currentCardData.damage + "花色颜色" + currentCardData.suitType + "是否暴击：" + currentCardData.isCritical);
            currentCardData.isCritical = CheckCriticalHit();
            var card = cardManager.GetCardObject().GetComponent<CardEntity>();
            card.Init(currentCardData);
            handleCardObjectList.Add(card);
        }
        SortHandCards();
    }
    /// <summary>
    /// 概率抽暴击牌
    /// </summary>
    /// <returns></returns>
    private bool CheckCriticalHit()
    {
        return Random.Range(0f, 1f) <= playerState.currentCritical / 100f;
    }

    //洗牌函数
    private void ShuffleDeck()
    {
        //清空弃牌堆
        discardDeck.Clear();
        //更新UI数量
        for (int i = 0; i < drawDeck.Count; i++)
        {
            //洗牌
            CardData temp = drawDeck[i];
            //随机生成一个索引
            int randomIndex = Random.Range(i, drawDeck.Count);
            //将当前索引的牌与随机索引的牌交换位置
            drawDeck[i] = drawDeck[randomIndex];
            drawDeck[randomIndex] = temp;
        }

    }
    //设置牌的位置
    private void SetCardLayout()
    {
        for (int i = 0; i < handleCardObjectList.Count; i++)
        {
            CardEntity currentCard = handleCardObjectList[i];
            CardTransfrom currentCardTransfrom = cardLayoutManager.GetCardTransfrom(i, handleCardObjectList.Count);
            currentCard.canClick = false;
            currentCard.transform.DOScale(Vector3.one, 0.05f).onComplete = () =>//在动画执行完成后执行的内容
            {
                //卡牌位置移动到指定位置，使用DO实现动画
                currentCard.transform.DOMove(currentCardTransfrom.pos, 0.5f).onComplete = () => currentCard.canClick = true;
            };
        }
    }
    //排序手牌
    public void SortHandCards()
    {
        switch (sortingMode)
        {
            case SortingMode.ByRank:
                handleCardObjectList = handleCardObjectList
                .OrderBy(c => c.cardData.rank)
                .ThenBy(c => c.cardData.suit)
                .ToList();
                break;
            case SortingMode.BySuit:
                handleCardObjectList = handleCardObjectList
                .OrderBy(c => c.cardData.suit)
                .ThenBy(c => c.cardData.rank)
                .ToList();
                break;
        }
        foreach (var card in handleCardObjectList)
        {
            ReturnCardToHand(card);
        }
        SetCardLayout();
    }
    // 当卡片被点击时调用
    public void OnCardClicked(object card)
    {
        // 将点击的卡片转换为CardEntity类型
        CardEntity cardEntity = card as CardEntity;
        // 切换选中状态
        if (selectedCardList.Contains(cardEntity))
        {
            // 如果卡片已经在选中列表中，则将其返回到手牌中
            ReturnCardToHand(cardEntity);
            AudioManager.Instance.PlaySFX("CardCancel");
        }
        else
        {
            // 如果卡片不在选中列表中，则将其选中
            SelectCard(cardEntity);
        }
        // 触发选中事件
        selectedCardEvent.RaiseEvent(this, this);
    }

    // 将卡牌添加到选中堆中
    private void SelectCard(CardEntity card)
    {
        if (selectedCardList.Count > 4) return;
        AudioManager.Instance.PlaySFX("CardSelect");
        card.transform.DOMove(card.transform.position + new Vector3(0, 0.3f, 0), 0.2f);
        selectedCardList.Add(card);
        card.cardSelect.gameObject.SetActive(true);
        Debug.Log("选中：" + card.cardData + "选中数量：" + selectedCardList.Count);
    }
    // 将选中的牌返回到手牌中
    private void ReturnCardToHand(CardEntity card)
    {
        card.transform.DOMove(card.transform.position + new Vector3(0, -0.3f, 0), 0.2f);
        // 从选中的牌列表中移除该牌
        selectedCardList.Remove(card);
        // 取消该牌的选中状态
        card.cardSelect.gameObject.SetActive(false);
        // 输出取消选中的牌的信息
        Debug.Log("取消选中：" + card.cardData);
    }
    public void ByRank()
    {
        sortingMode = SortingMode.ByRank;
        SortHandCards();
    }
    public void BySuit()
    {
        sortingMode = SortingMode.BySuit;
        SortHandCards();
    }
    //丢弃选中的牌
    public void DiscardSelectedCards()
    {
        if (selectedCardList.Count < 1) return;
        // 遍历选中的牌列表
        foreach (var card in selectedCardList)
        {
            // 丢弃选中的牌
            DiscardCard(card);
        }
        // 根据丢弃的牌的数量，重新抽取牌
        DrawCard(maxCard - handleCardObjectList.Count);
        // 清空选中的牌列表
        selectedCardList.Clear();
        disSelectCardEvent.RaiseEvent(this, this);
    }
    //回收卡牌
    public void DiscardCard(object obj)
    {
        // 将obj转换为Card类型
        CardEntity card = obj as CardEntity;
        // 将卡牌数据添加到discardDeck中
        card.cardSelect.gameObject.SetActive(false);
        card.cardCritical.gameObject.SetActive(false);
        discardDeck.Add(card.cardData);
        // 从handleCardObjectList中移除卡牌
        handleCardObjectList.Remove(card);
        // 将卡牌缩放到0，持续0.2秒
        card.transform.DOScale(Vector3.zero, 0.2f).onComplete = () =>
        {
            // 回收卡牌对象
            //card.transform.DOMove(card.transform.position + new Vector3(0, -0.3f, 0), 0.2f);
            cardManager.ReturnCardObject(card.gameObject);
            card.transform.position = deckPos;
        };

    }
    //判断抽牌堆中是否包含某张牌
    public bool DrawDeckContains(CardSuit suit, CardRank rank)
    {
        return drawDeck.Exists(c => c.suit == suit && c.rank == rank);
    }


    //关闭手牌点击
    public void CloseCardClick()
    {
        foreach (var card in handleCardObjectList)
        {
            card.canClick = false;
        }
    }
    //开启手牌点击
    public void OpenCardClick()
    {
        foreach (var card in handleCardObjectList)
        {
            card.canClick = true;
        }
    }
    public void ResetDiscardNumber()
    {
        discardNumber = MaxDiscardNumber;
        discardNumberEvent.RaiseEvent(discardNumber, this);
    }
    public void AddDiscardNumber()
    {
        discardNumber++;
        discardNumberEvent.RaiseEvent(discardNumber, this);
    }
    public void ReduceDiscardNumber()
    {
        Debug.Log("discardNumber:" + discardNumber + "selectedCardList.Count:" + selectedCardList.Count);
        if (selectedCardList.Count < 1) return;
        discardNumber--;
        discardNumberEvent.RaiseEvent(discardNumber, this);
    }

}
