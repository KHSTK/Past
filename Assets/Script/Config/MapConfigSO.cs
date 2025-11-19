using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Config/MapConfigSO")]
public class MapConfigSO : ScriptableObject
{
    public List<RoomBlueprint> roomBlueprints;
}
[System.Serializable]
public class RoomBlueprint
{
    public int min, max;
    public RoomType roomType;
}
