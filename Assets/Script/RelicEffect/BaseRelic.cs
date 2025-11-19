using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public abstract class BaseRelic : ScriptableObject
{
    public bool isDamageRelic;
    public abstract bool ApplyEffect(DamageContext ctx);
}
