using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DataSO/Room State")]
public class CurrentRoomStateSO : ScriptableObject
{
    [Header("运行时数据")]
    public RoomData currentRoomData;
    public RoomEntity currentRoomEntity;

    [Header("配置数据")]
    public RoomDataBase roomDataBase;

    public void SetCurrentRoom(RoomData roomData, RoomEntity roomEntity)
    {
        currentRoomData = roomData;
        currentRoomEntity = roomEntity;
        Debug.Log("当前房间：" + currentRoomData.roomType);
    }

    public void ClearCurrentRoom()
    {
        currentRoomData = null;
    }
}
