using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEffect : ScriptableObject
{
    [TextArea] public string description;
    public virtual void ApplyEffect(DamageContext ctx)
    {

    }
}
