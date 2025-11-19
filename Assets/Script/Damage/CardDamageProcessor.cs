using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDamageProcessor : MonoBehaviour, IDamageProcessor
{
    public int ExecutionOrder => 100;
    public DamagePopupEventSO damageEventSO;
    private DamagePopup damagePopup;

    // 执行DamageContext中的DamageProcessor
    public IEnumerator Execute(DamageContext ctx)
    {
        // 遍历ctx.playedCards中的每个card
        foreach (var card in ctx.playedCards)
        {
            // 将card.cardData.damage加到ctx.currentDamage上
            ctx.currentDamage += card.cardData.damage;
            // 打印card.cardData.damage
            Debug.Log("CardDamageProcessor: " + card.cardData.damage);
            // 创建DamagePopup对象
            damagePopup = new DamagePopup
            {
                // 设置DamagePopup的位置为card的位置
                position = new Vector3(card.transform.position.x, card.transform.position.y + 1f, card.transform.position.z - 1),
                // 设置DamagePopup的damage为card.cardData.damage
                popupText = card.cardData.damage.ToString(),
                damageContext = ctx
            };
            // 触发damageEventSO事件
            damageEventSO.RaiseEvent(damagePopup, this);
            // 等待0.3秒
            yield return new WaitForSeconds(0.3f);
        }
    }


}
