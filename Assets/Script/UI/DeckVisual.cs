using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckVisual : MonoBehaviour
{
    [SerializeField] private CardDatabase cardDatabase;
    [SerializeField] private CardDeck cardDeck;
    [SerializeField] private Transform cardGrid;
    [SerializeField] private GameObject cardUIPrefab;
    private Dictionary<(CardSuit, CardRank), UICard> uiCardDict = new();
    private void Start()
    {
        InitializeCardGrid();
    }
    private void InitializeCardGrid()
    {
        // 按花色和点数排序
        var sortedData = cardDatabase.cardVisualData.OrderBy(d => d.suit).ThenBy(d => (int)d.rank);

        foreach (var visualData in sortedData)
        {
            var uiCard = Instantiate(cardUIPrefab, cardGrid).GetComponent<UICard>();
            uiCard.Init(visualData, cardDatabase);
            uiCardDict[(visualData.suit, visualData.rank)] = uiCard;
        }
    }
    public void UpdataAllUICardState()
    {
        foreach (var uiCard in uiCardDict)
        {
            var cardrank = uiCard.Key.Item1;
            var cardsuit = uiCard.Key.Item2;
            uiCard.Value.UpdateState(cardDeck.DrawDeckContains(cardrank, cardsuit));
        }
    }
}
