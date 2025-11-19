using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICard : MonoBehaviour
{
    [SerializeField] private Image cardSprite;
    [SerializeField] private Color color;
    public void Init(CardDatabase.CardVisualData cardData, CardDatabase cardDatabase)
    {
        cardSprite.sprite = cardDatabase.GetCardSprite(cardData.suit, cardData.rank);
    }
    public void UpdateState(bool ishave)
    {
        if (!ishave)
        {
            cardSprite.color = color;
        }
        else
        {
            cardSprite.color = Color.white;
        }
    }
}
