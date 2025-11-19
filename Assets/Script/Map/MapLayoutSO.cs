using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "MapLayoutSO", menuName = "Map/MapLayoutSO", order = 0)]
public class MapLayoutSO : ScriptableObject
{
    public List<MapRoomData> mapRoomDataList = new();
    public List<LinePos> mapLineDataList = new();

}

[System.Serializable]
//房间地图数据类
public class MapRoomData
{
    //房间在地图上的X坐标
    public float posX, posY;
    //房间在地图上的列数
    public int column, line;
    //房间类型
    public RoomType roomType;
    //房间状态
    public RoomState roomState;
    //房间连接的房间列表
    public List<Vector2Int> linkTo = new();
}

[System.Serializable]
//连线数据
public class LinePos
{
    public SerializeVector3 startPos, endPos;
}