using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Relic/HaveFlush")]

public class HaveFlush : BaseRelic
{
    [SerializeField] private int relicDamage;
    public override bool ApplyEffect(DamageContext ctx)
    {
        if (ctx.comboType == ComboType.Flush || ctx.comboType == ComboType.StraightFlush)
        {
            ctx.relicDamage = relicDamage;
            ctx.currentDamage += ctx.relicDamage;
            return true;
        }
        else
        {
            return false;
        }
    }
}
