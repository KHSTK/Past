using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Relic/HaveThree")]

public class HaveThree : BaseRelic
{
    [SerializeField] private int relicDamage;
    public override bool ApplyEffect(DamageContext ctx)
    {
        if (ctx.comboType == ComboType.Three || ctx.comboType == ComboType.FullHouse)
        {
            ctx.relicDamage = relicDamage;
            ctx.currentDamage += ctx.relicDamage;
            Debug.Log("HaveThree");
            return true;
        }
        else
        {
            return false;
        }
    }
}
