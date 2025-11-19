using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RoomData
{
    public int column;
    public int line;
    public RoomType roomType;
    public EnemyData enemyData;  // 敌人数据
    public RoomEventData eventData;// 事件数据
    //不注入默认为空
    public RoomData(int column, int line, RoomType roomType,
        EnemyData enemyData = null, RoomEventData eventData = null)
    {
        this.column = column;
        this.line = line;
        this.roomType = roomType;
        this.enemyData = enemyData;
        this.eventData = eventData;
    }
    public string GetRoomName()
    {
        return roomType switch
        {
            RoomType.Normal or RoomType.Elite or RoomType.Boss => enemyData.enemyName,
            RoomType.Event => "回廊",
            RoomType.Shop => "交易处",
            RoomType.Rest => "联络处",
            _ => "未知房间"
        };
    }
    public string GetRoomDescription()
    {
        return roomType switch
        {
            RoomType.Normal or RoomType.Elite or RoomType.Boss => enemyData.enemyDescription,
            RoomType.Event => "回廊上传来嘈杂的异响……",
            RoomType.Shop => "这里没有信使，只有那个在腐烂树洞里兜售邮票的“末驿商人”。",
            RoomType.Rest => "这里聚集了一群向导鼠",
            _ => "未知房间"
        };
    }
}
