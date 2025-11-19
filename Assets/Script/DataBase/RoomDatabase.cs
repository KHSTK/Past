using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Database/RoomDataBase")]
public class RoomDataBase : ScriptableObject
{
    public List<RoomVisualData> roomVisualData = new List<RoomVisualData>();
    [System.Serializable]
    public class RoomVisualData
    {
        public RoomType roomType;
        public Sprite roomSprite;
    }
    public Sprite GetRoomSprite(RoomType roomType)
    {
        return roomVisualData.Find(x => x.roomType == roomType).roomSprite;
    }
}
