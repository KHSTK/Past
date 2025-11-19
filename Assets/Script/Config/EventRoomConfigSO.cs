
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Config/EventRoomConfig")]
public class EventRoomConfigSO : ScriptableObject
{
    public List<RoomEventData> events = new List<RoomEventData>();
    /// <summary>
    /// 随机获取一个事件
    /// </summary>
    /// <returns></returns>
    public RoomEventData GetRandomEvent()
    {
        return events.Count > 0 ? events[Random.Range(0, events.Count)] : null;
    }
}

[System.Serializable]
public class RoomEventData
{
    public string eventID;
    public string eventName;
    [TextArea] public string startDescription; // 事件开始描述
    public List<EventOption> options = new List<EventOption>(); // 事件选项
}
[System.Serializable]
public class EventOption
{
    public string optionName;
    public List<EventEffect> effects;
    [TextArea] public string endDescription; // 事件结束描述
}
[System.Serializable]
public class EventEffect
{
    public EffectType effectType;
    public int intValue;      // 用于数值型效果
    public string stringValue;
}