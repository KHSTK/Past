using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CardManager : MonoBehaviour
{
    public PoolTool poolTool;
    [Header("卡牌库")]
    public List<CardData> allCardList = new();
    public List<CardData> cardList = new();
    public CardDatabase cardDatabase;
    void Awake()
    {
        Init();
    }
    public void Init()
    {
        foreach (CardSuit suit in System.Enum.GetValues(typeof(CardSuit)))
        {
            foreach (CardRank rank in System.Enum.GetValues(typeof(CardRank)))
            {
                allCardList.Add(new CardData(suit, rank, cardDatabase));
            }
        }
    }
    public GameObject GetCardObject()
    {
        return poolTool.GetGameObjectFromPool();
    }
    /// <summary>
    /// 回收卡牌
    /// </summary>
    /// <param name="cardObject">回收对象</param>
    public void ReturnCardObject(GameObject cardObject)
    {
        poolTool.ReleaseGameObjectToPool(cardObject);
    }
}
