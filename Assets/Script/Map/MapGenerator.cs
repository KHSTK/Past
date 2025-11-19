using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics.Internal;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("浮现动画设置")]
    [SerializeField] private float roomAppearDelay = 0.1f;
    [SerializeField] private float lineAppearDelay = 0.3f;
    [Header("连线参数")]
    [SerializeField] private float curveHeight = 2f; // 弧线高度系数
    [SerializeField] private int segmentCount = 20; // 曲线细分段数
    [Header("地图配置表")]
    public MapConfigSO mapConfig;
    [Header("房间配置")]
    public EnemyConfigSO enemyConfig;
    public EventRoomConfigSO eventRoomConfig;
    public CurrentRoomStateSO currentRoomState;
    [Header("预制体")]
    public RoomEntity roomPrefab;
    public LineRenderer linePrefab;
    [Header("地图参数")]
    public GameObject Map;
    public int seed;
    public bool useRandomSeed;
    public float initialHeightOffset;
    [Header("数据保存")]
    public MapSaveDataSO mapSaveData;
    private Vector2Int clickedRoomPos;
    private float screenHeight;
    private float screenWidth;
    private float columnWidth;
    private Vector3 generatePoint;
    public float border;
    private List<RoomEntity> rooms = new();
    private List<LineRenderer> lines = new();
    //坐标房间对应字典，用于更新状态
    public Dictionary<Vector2Int, RoomEntity> roomMap = new Dictionary<Vector2Int, RoomEntity>();
    void Awake()
    {
        screenHeight = Camera.main.orthographicSize * 2 - 1;
        screenWidth = screenHeight * Camera.main.aspect;
        columnWidth = screenWidth / (mapConfig.roomBlueprints.Count + 0.5f);
    }

    private void OnEnable()
    {
        //CreateMap();
    }
    private void InitializedSeed()
    {
        if (useRandomSeed)
        {
            seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }
        UnityEngine.Random.InitState(seed);
    }

    public void CreateMap()
    {
        InitializedSeed();
        Debug.Log("seed:" + seed);
        //创建前一列房间的列表
        List<RoomEntity> previousColumn = new List<RoomEntity>();
        //循环生成每列房间
        for (int column = 0; column < mapConfig.roomBlueprints.Count; column++)
        {
            var blueprint = mapConfig.roomBlueprints[column];
            //随机房间数
            var amount = UnityEngine.Random.Range(blueprint.min, blueprint.max + 1);
            float startHeight = 0;
            //每行初始房间高
            if (amount < 2)
            {
                startHeight = screenHeight / 2 - screenHeight / (amount + 1);
            }
            else
            {

                startHeight = screenHeight * (0.5f - initialHeightOffset);
            }
            //每行初始房间坐标generatePoint
            generatePoint = new Vector3(-screenWidth / 2 + border + column * columnWidth, startHeight, 0);
            //每个房间坐标
            var newPosition = generatePoint;
            //当前列房间列表
            List<RoomEntity> currentColumn = new List<RoomEntity>();
            //每行间距
            var roomGapY = screenHeight / (amount + 1) * 0.6f;

            //循环生成每行房间
            for (int i = 0; i < amount; i++)
            {

                //最后一个房间固定坐标
                if (column == mapConfig.roomBlueprints.Count - 1)
                {
                    newPosition.x = screenWidth / 2 - border * 2;
                    newPosition.y = startHeight - roomGapY * i + 0.7f;
                }
                //随机偏移
                else if (column != 0)
                {
                    //newPosition.x = generatePoint.x + UnityEngine.Random.Range(-border / 2, border / 2);
                    newPosition.y = startHeight - roomGapY * i;
                }
                else
                {
                    newPosition.y = startHeight - roomGapY * i;
                }
                //生成房间
                var room = Instantiate(roomPrefab, newPosition, Quaternion.identity, Map.transform);
                //设置房间类型
                RoomType newType = GetRandomRoomType(mapConfig.roomBlueprints[column].roomType);
                if (column == 0)
                {
                    room.roomState = RoomState.Attainable;
                }
                else
                {
                    room.roomState = RoomState.Locked;
                }
                //初始化房间数据
                EnemyData selectedEnemy = null;
                RoomEventData selectedEvent = null;
                // 根据房间类型配置数据
                switch (newType)
                {
                    case RoomType.Normal:
                        selectedEnemy = enemyConfig.GetRandomEnemyDataByType(EnemyType.Normal);
                        break;
                    case RoomType.Elite:
                        selectedEnemy = enemyConfig.GetRandomEnemyDataByType(EnemyType.Elite);
                        break;
                    case RoomType.Boss:
                        selectedEnemy = enemyConfig.GetRandomEnemyDataByType(EnemyType.Boss);
                        break;
                    case RoomType.Event:
                        selectedEvent = eventRoomConfig.GetRandomEvent();
                        break;
                    case RoomType.Rest:
                        break;
                }
                //RoomData初始化
                var roomData = new RoomData(column, i, newType, selectedEnemy, selectedEvent);
                room.SetupRoom(roomData);
                // 记录到字典
                Vector2Int posKey = new Vector2Int(column, i);
                roomMap[posKey] = room;
                //记录到列表
                rooms.Add(room);
                currentColumn.Add(room);
            }
            //判断是否为第一列，如果不是则连接到上一列
            if (previousColumn.Count > 0)
            {
                //创建房间连线
                ConnectRooms(previousColumn, currentColumn);
            }
            //记录当前列
            previousColumn = currentColumn;
        }
        SaveMapData();
        StartCoroutine(PlayAppearAnimations());
    }
    private IEnumerator PlayAppearAnimations()
    {
        // 播放所有房间动画（线段保持禁用）
        foreach (RoomEntity room in rooms)
        {
            room.gameObject.SetActive(true);
            room.PlayAppearAnimation();
            yield return new WaitForSeconds(roomAppearDelay);
        }

        // 等待所有房间动画完成
        yield return new WaitForSeconds(lineAppearDelay);

        // 播放所有线段动画
        foreach (LineRenderer line in lines)
        {
            Line lineComponent = line.GetComponent<Line>();
            if (lineComponent != null)
            {
                lineComponent.PlayLineReveal();
                //yield return new WaitForSeconds(lineAppearDelay * 0.3f);
            }
        }
    }
    /// <summary>
    /// 连接两个房间的函数
    /// </summary>
    /// <param name="column1">第一列的所有房间</param>
    /// <param name="column2">第二列的所有房间</param>
    private void ConnectRooms(List<RoomEntity> column1, List<RoomEntity> column2)
    {
        // 初始化下一个房间索引
        int nextRoom = 0;
        // 遍历第一个房间的列表
        for (int i = 0; i < column1.Count; i++)
        {
            // 计算最大连接数
            int maxConnections = column2.Count - nextRoom - 1;
            // 随机生成连接数
            int randomConnections = UnityEngine.Random.Range(1, Mathf.Max(1, maxConnections));
            // 如果是最后一个房间，则连接到所有剩余的房间
            if (i == column1.Count - 1)
            {
                randomConnections = column2.Count - nextRoom;
            }
            // 遍历连接数
            for (int j = nextRoom; j < nextRoom + randomConnections; j++)
            {
                var line = Instantiate(linePrefab, transform);
                line.gameObject.SetActive(false); // 初始状态设为禁用
                List<Vector3> curvePoints = CalculateCurve(
                    column1[i].transform.position,
                    column2[j].transform.position
                );

                line.positionCount = curvePoints.Count;
                line.SetPositions(curvePoints.ToArray());
                // 调整材质平铺
                line.material.mainTextureScale = new Vector2(
                    curvePoints.Count / 5f,
                    1
                );
                lines.Add(line);
                column1[i].linkTo.Add(new(column2[j].roomData.column, column2[j].roomData.line));
            }
            // 更新下一个房间索引
            nextRoom = Mathf.Min(nextRoom + randomConnections, column2.Count - 1);
        }
    }
    [ContextMenu(itemName: "ReGenerateRoom")]
    //重新生成地图
    public void ReGenerateRoom()
    {
        Debug.Log("重新生成地图");
        StopAllCoroutines();
        //销毁游戏物体
        foreach (var room in rooms)
        {
            Destroy(room.gameObject);
        }
        foreach (var line in lines)
        {
            Destroy(line.gameObject);
        }
        //清空列表
        lines.Clear();
        rooms.Clear();
        roomMap.Clear();
        CreateMap();
    }

    /// <summary>
    /// 根据枚举随机返回一个房间类型
    /// </summary>
    /// <param name="flages"></param>
    /// <returns></returns>
    private RoomType GetRandomRoomType(RoomType flages)
    {
        string[] options = flages.ToString().Split(',');
        string randomOption = options[UnityEngine.Random.Range(0, options.Length)];
        RoomType roomType = (RoomType)Enum.Parse(typeof(RoomType), randomOption);
        return roomType;
    }
    public void GetCurrentRoom()
    {
        if (currentRoomState.currentRoomData == null) return;
        clickedRoomPos = new Vector2Int(currentRoomState.currentRoomData.column, currentRoomState.currentRoomData.line);
    }
    // 房间状态更新方法
    public void UpdateRoomStates()
    {
        // 通过坐标获取房间实例
        if (!roomMap.TryGetValue(clickedRoomPos, out RoomEntity clickedRoom))
        {
            Debug.Log($"无法找到坐标 {clickedRoomPos} 对应的房间");
            return;
        }
        //标记当前房间为已访问
        clickedRoom.roomState = RoomState.Visited;
        clickedRoom.UpdateRoomStateVisual();

        //找到所有可到达的房间
        HashSet<Vector2Int> attainableRooms = new HashSet<Vector2Int>();
        foreach (var link in clickedRoom.linkTo)
        {
            Vector2Int linkedPos = new Vector2Int(link.x, link.y);
            if (roomMap.TryGetValue(linkedPos, out RoomEntity room))
            {
                attainableRooms.Add(linkedPos);
            }
        }

        //更新所有房间状态
        foreach (var entry in roomMap)
        {
            RoomEntity room = entry.Value;

            if (room.roomState == RoomState.Visited) continue;

            if (attainableRooms.Contains(entry.Key))
            {
                room.roomState = RoomState.Attainable;
            }
            else
            {
                room.roomState = RoomState.Locked;
            }
            room.UpdateRoomStateVisual();
        }
        SaveMapData();
    }
    //保存地图数据
    public void SaveMapData()
    {
        mapSaveData.seed = seed;
        foreach (var entry in roomMap)
        {
            mapSaveData.rooms.Add(new RoomSaveData()
            {
                column = entry.Key.x,
                line = entry.Key.y,
                state = entry.Value.roomState,
            });
        }
    }
    //加载地图数据
    public void LoadMapData()
    {
        useRandomSeed = false;
        seed = mapSaveData.seed;
        CreateMap();
        foreach (var roomSave in mapSaveData.rooms)
        {
            Vector2Int pos = new Vector2Int(roomSave.column, roomSave.line);
            if (roomMap.TryGetValue(pos, out RoomEntity room))
            {
                room.roomState = roomSave.state;
                room.UpdateRoomStateVisual();
            }
        }
    }
    /// <summary>
    /// 房间连线
    /// </summary>
    /// <param name="start">起始点</param>
    /// <param name="end">终点</param>
    /// <returns></returns>
    private List<Vector3> CalculateCurve(Vector3 start, Vector3 end)
    {
        List<Vector3> points = new List<Vector3>();

        // 计算控制点（中间点上方偏移）
        Vector3 controlPoint = (start + end) / 2;
        controlPoint += Vector3.up * Vector3.Distance(start, end) * curveHeight;

        // 生成贝塞尔曲线点
        for (int i = 0; i <= segmentCount; i++)
        {
            float t = i / (float)segmentCount;
            points.Add(CalculateBezierPoint(t, start, controlPoint, end));
        }

        return points;
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // 二次贝塞尔曲线公式
        float u = 1 - t;
        return u * u * p0 +
               2 * u * t * p1 +
               t * t * p2;
    }
}





