using System;
using System.Text.RegularExpressions;
using UnityEngine;
[CreateAssetMenu(menuName = "EnemyEffect/LowCardEffect")]
public class LowCardEffect : EnemyEffect
{
    public float reduction;
    public override void ApplyEffect(DamageContext ctx)
    {
        if (ctx.playedCards.Count < 3)
        {
            ctx.currentDamage = (int)Math.Ceiling(ctx.currentDamage * (1 - reduction));
        }
    }
}
