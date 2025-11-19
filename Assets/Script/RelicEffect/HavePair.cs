using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Relic/HavePair")]
public class HavePair : BaseRelic
{
    [SerializeField] private int relicDamage;
    public override bool ApplyEffect(DamageContext ctx)
    {
        if (ctx.comboType == ComboType.Pair || ctx.comboType == ComboType.TwoPairs || ctx.comboType == ComboType.FullHouse)
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
