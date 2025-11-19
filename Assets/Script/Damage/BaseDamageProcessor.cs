using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 基础伤害处理器
/// </summary>
public class BaseDamageProcessor : MonoBehaviour, IDamageProcessor
{

    public int ExecutionOrder => 0;

    public IEnumerator Execute(DamageContext ctx)
    {
        ctx.currentDamage = ctx.baseDamage;
        Debug.Log("BaseDamageProcessor" + ctx.currentDamage);
        yield return null;
    }
}
