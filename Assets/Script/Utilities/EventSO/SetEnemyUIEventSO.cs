using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SetEnemyUIEventSO", menuName = "Events/SetEnemyUIEventSO")]
public class SetEnemyUIEventSO : BaseEventSO<EnemyState>
{

}
public struct EnemyState
{
    public string enemyName;
    public string currentHp;
    public string currentDamage;
    public string currentTime;
    public Sprite enemySprite;
}

