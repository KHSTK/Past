using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Character : MonoBehaviour
{
    public int HP { get; protected set; }
    public bool isAlive => HP > 0;

    protected void Initialize(int maxHp)
    {
        HP = maxHp;
    }

    protected void ReduceHP(int amount)
    {
        HP = Mathf.Max(HP - amount, 0);
    }

    public abstract void TakeDamage(int damage);
}