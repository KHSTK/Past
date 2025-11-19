using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Relic/HaveSingle")]

public class HaveSingle : BaseRelic
{
    [SerializeField] private int relicDamage;
    public override bool ApplyEffect(DamageContext ctx)
    {
        if (ctx.comboType == ComboType.Single)
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
