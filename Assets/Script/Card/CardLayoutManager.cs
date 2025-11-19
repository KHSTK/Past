using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardLayoutManager : MonoBehaviour
{
    public float cardSpace = 0.5f;//卡牌间隙
    public Vector3 centerPoint;//卡牌中心点
    private List<Vector3> cardPos = new();//卡牌位置
    /// <summary>
    /// 获取卡牌位置
    /// </summary>
    /// <param name="index">第几张牌</param>
    /// <param name="cardNum">总共几张牌</param>
    /// <returns></returns>
    public CardTransfrom GetCardTransfrom(int index, int cardNum)
    {
        CalculateCardPos(cardNum);
        return new CardTransfrom(cardPos[index]);
    }

    /// <summary>
    /// 计算卡牌位置
    /// </summary>
    /// <param name="cardNum">卡牌数量</param>
    private void CalculateCardPos(int cardNum)
    {
        cardPos.Clear();
        float currentWidth = (cardNum - 1) * cardSpace;//卡牌总宽度
        float currentSpacing = currentWidth / (cardNum - 1);//卡牌间隙
        for (int i = 0; i < cardNum; i++)
        {
            float xPos = centerPoint.x - (currentWidth / 2) + (i * currentSpacing);
            var pos = new Vector3(xPos, centerPoint.y, 0f);
            cardPos.Add(pos);
        }
    }
}
