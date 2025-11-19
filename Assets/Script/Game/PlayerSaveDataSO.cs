using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "SaveData/PlayerSaveDataSO")]
public class PlayerSaveDataSO : ScriptableObject
{
    public int HP;
    public int coin;
    public int critical;
}
