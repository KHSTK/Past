using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComprehensiveTestRoomGenerator : MonoBehaviour
{
    [Header("资源引用")]
    public RoomEntity roomPrefab;
    public EnemyConfigSO enemyConfig;
    public EventRoomConfigSO eventRoomConfig;
    public RoomDataBase roomDataBase;
    public CurrentRoomStateSO currentRoomState;
    public ObjectEventSO clickRoomEvent;
    private List<RoomEntity> roomList = new List<RoomEntity>();
    private float roomIndex = -5, roomCount = 3;
    private void Awake()
    {
        CreateAllRooms();
    }
    private void CreateAllRooms()
    {
        var roomData = new RoomData(-1, -1, RoomType.Normal, null, null);
        foreach (var enemyData in enemyConfig.enemyDataList)
        {
            if (enemyData.enemyType == EnemyType.Normal)
            {
                roomData = new RoomData(-1, -1, RoomType.Normal, enemyData, null);
            }
            else if (enemyData.enemyType == EnemyType.Elite)
            {
                roomData = new RoomData(-1, -1, RoomType.Elite, enemyData, null);
            }
            else
            {
                roomData = new RoomData(-1, -1, RoomType.Boss, enemyData, null);
            }
            CreateRoom(roomData);
        }
        foreach (var eventData in eventRoomConfig.events)
        {
            roomData = new RoomData(-1, -1, RoomType.Event, null, eventData);
            CreateRoom(roomData);
        }
        CreateRoom(new RoomData(-1, -1, RoomType.Shop, null, null));
        CreateRoom(new RoomData(-1, -1, RoomType.Rest, null, null));

    }
    private void CreateRoom(RoomData roomData)
    {
        roomIndex += 2f;
        if (roomIndex > 5)
        {
            roomCount -= 2f;
            roomIndex = -5f;
        }
        var room = Instantiate(roomPrefab, new Vector3(roomIndex, roomCount, 0), Quaternion.identity, transform);
        room.SetupRoom(roomData);
        roomList.Add(room);
        room.PlayAppearAnimation();
    }
}
