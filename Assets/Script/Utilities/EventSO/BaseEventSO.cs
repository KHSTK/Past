using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class BaseEventSO<T> : ScriptableObject
{
    //事件描述
    [TextArea]
    public string description;
#if UNITY_EDITOR
    [Space(20)]
    [SerializeField] private List<string> _senders = new List<string>();
    [SerializeField] private int _maxDisplay = 5; // 控制最大显示数量
#endif
    public UnityAction<T> OnEventRaised;

    public string lastSender;
    // 事件触发方法
    public void RaiseEvent(T value, object sender)
    {
        // 如果事件不为空，则触发事件
        OnEventRaised?.Invoke(value);
        // 将最后一个触发事件的发送者保存到lastSender变量中
#if UNITY_EDITOR
        RecordSender(sender.ToString());
#endif
    }
#if UNITY_EDITOR
    /// <summary>
    /// 记录发送者信息的方法
    /// </summary>
    /// <param name="sender">要记录的发送者名称</param>
    private void RecordSender(string sender)
    {
        // 保持列表长度不超过最大显示数量
        while (_senders.Count >= _maxDisplay)
        {
            // 当列表长度超过限制时，移除最早添加的记录（FIFO）
            _senders.RemoveAt(0);
        }

        // 添加新的发送者记录，包含发送者名称和当前时间（格式：HH:mm:ss）
        _senders.Add($"{sender} ({System.DateTime.Now:HH:mm:ss})");
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif

}
