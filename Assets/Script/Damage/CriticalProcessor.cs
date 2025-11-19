using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class CriticalProcessor : MonoBehaviour, IDamageProcessor
{
    public DamagePopupEventSO damageEventSO;
    private DamagePopup damagePopup;
    public int ExecutionOrder => 300;

    public IEnumerator Execute(DamageContext ctx)
    {
        yield return new WaitForSeconds(0.3f);
        ctx.currentDamage = Mathf.RoundToInt(ctx.currentDamage * (1 + 0.25f * ctx.criticalCount));
        foreach (var card in ctx.playedCards)
        {
            if (card.cardData.isCritical)
            {
                damagePopup = new DamagePopup
                {
                    // 设置DamagePopup的位置为card的位置
                    position = new Vector3(card.transform.position.x, card.transform.position.y + 1.5f, card.transform.position.z - 1),
                    popupText = "25%",
                    damageContext = ctx
                };
                damageEventSO.RaiseEvent(damagePopup, this);
            }
        }
        Debug.Log($"CriticalProcessor: " + ctx.currentDamage);
        yield return new WaitForSeconds(0.3f);
    }
}
