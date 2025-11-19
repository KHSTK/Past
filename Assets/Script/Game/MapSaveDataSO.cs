using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "SaveData/MapSaveData")]
public class MapSaveDataSO : ScriptableObject
{
    public int seed;
    public List<RoomSaveData> rooms = new List<RoomSaveData>();
}
[System.Serializable]
public class RoomSaveData
{
    public int column;
    public int line;
    public RoomState state;
    public RoomSaveData() { }
}
