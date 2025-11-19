using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "DataSO/PlayerState")]
public class PlayerStateSO : ScriptableObject
{
    public int maxHp;
    public int currentHp;
    public int currentCoin;
    public int currentCritical;
    public void AddHp(int hp)
    {
        currentHp = Mathf.Clamp(currentHp + hp, 0, maxHp);
    }
    public void AddCoin(int coin)
    {
        currentCoin = Mathf.Max(currentCoin + coin, 0);
    }
    public void AddCritical(int critical)
    {
        currentCritical = Mathf.Max(currentCritical + critical, 0);
    }
}
