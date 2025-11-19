using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardEntity : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("广播")]
    public ObjectEventSO onCardClick;
    [Header("卡牌数据")]
    public CardData cardData;
    public bool canClick;
    [Header("卡牌卡面")]
    public SpriteRenderer cardSprite;
    public SpriteRenderer cardSelect;
    public SpriteRenderer cardCritical;
    public CardDatabase cardDatabase;
    public GameObject entity;
    public void Init(CardData cardData)
    {
        this.cardData = cardData;
        UpdataVisual();
    }
    public void UpdataVisual()
    {
        //根据卡牌数据更新卡牌的显示
        if (cardData.isCritical) cardCritical.gameObject.SetActive(true);
        else cardCritical.gameObject.SetActive(false);
        cardSprite.sprite = cardDatabase.GetCardSprite(cardData.suit, cardData.rank);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!canClick) return;
        //鼠标进入卡牌时
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!canClick) return;
        //鼠标离开卡牌时
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canClick) return;
        //鼠标点击卡牌时
        onCardClick.RaiseEvent(this, this);
    }
    public void Attack()
    {
        transform.DOMove(new Vector3(0, 1.5f, 0), 0.3f);
        transform.DOScale(Vector3.zero, 0.3f);
    }
}
