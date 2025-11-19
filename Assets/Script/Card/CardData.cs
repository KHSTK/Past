using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CardData
{
    [SerializeField]
    public CardSuit suit;
    public CardRank rank;
    public SuitType suitType;
    public Sprite cardSprite;
    public int damage;
    public bool isCritical; // 是否为暴击牌

    public CardData(CardSuit suit, CardRank rank, CardDatabase db)
    {
        this.suit = suit;
        this.rank = rank;
        cardSprite = db.GetCardSprite(suit, rank);
        SetSuitType();
        damage = (int)rank;
    }
    public void SetSuitType()
    {
        switch (suit)
        {
            case CardSuit.Diamond:
                suitType = SuitType.Red;
                break;
            case CardSuit.Heart:
                suitType = SuitType.Red;
                break;
            case CardSuit.Club:
                suitType = SuitType.Black;
                break;
            case CardSuit.Spade:
                suitType = SuitType.Black;
                break;
        };
    }
}
