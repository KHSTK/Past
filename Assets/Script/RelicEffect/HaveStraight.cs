using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Relic/HaveStraight")]
public class HaveStraight : BaseRelic
{
    [SerializeField] private int relicDamage;
    public override bool ApplyEffect(DamageContext ctx)
    {
        if (ctx.comboType == ComboType.Straight || ctx.comboType == ComboType.StraightFlush)
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
