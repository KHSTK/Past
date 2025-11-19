using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RelicProcessor : MonoBehaviour, IDamageProcessor
{
    public RelicManager RelicManager;
    public DamagePopupEventSO damagePopupEvent;
    public int ExecutionOrder => 200;

    public IEnumerator Execute(DamageContext ctx)
    {
        // 仅处理伤害相关的Relic
        var displayRelics = RelicManager.activeRelicMap.Keys
            .Where(r => r.isDamageRelic);
        yield return new WaitForSeconds(0.3f);
        foreach (var relic in displayRelics)
        {
            if (relic.ApplyEffect(ctx))
            {
                Vector3 popupPos = RelicManager.GetRelicWorldPosition(relic);
                var damagePopup = new DamagePopup
                {
                    popupText = ctx.relicDamage.ToString(),
                    //position = new Vector3(RelicManager.transform.position.x - 1.2f, RelicManager.transform.position.y, RelicManager.transform.position.z - 1),
                    position = popupPos,
                    damageContext = ctx
                };
                damagePopupEvent.RaiseEvent(damagePopup, this);
                yield return new WaitForSeconds(0.3f);
            }
        }
    }
}
