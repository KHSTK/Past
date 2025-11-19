using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Database/CardDatabase")]
public class CardDatabase : ScriptableObject
{
    public List<CardVisualData> cardVisualData = new List<CardVisualData>();
    [System.Serializable]
    public class CardVisualData
    {
        public CardSuit suit;
        public CardRank rank;
        public Sprite sprite;
    }
    public Sprite GetCardSprite(CardSuit suit, CardRank rank)
    {
        return cardVisualData.Find(v => v.suit == suit && v.rank == rank).sprite;
    }
}
