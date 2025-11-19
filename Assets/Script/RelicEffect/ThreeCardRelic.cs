using UnityEngine;
[CreateAssetMenu(menuName = "Relic/ThreeCardRelic")]
public class ThreeCardRelic : BaseRelic
{
    [SerializeField] private int relicDamage;
    public override bool ApplyEffect(DamageContext ctx)
    {
        if (ctx.playedCards.Count >= 3)
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
