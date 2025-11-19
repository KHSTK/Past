using System;
using UnityEngine;
using UnityEngine.Events;

public class BaseEventListener<T> : MonoBehaviour
{
    public BaseEventSO<T> eventSO;
    public UnityEvent<T> responseEvent;

    private void OnEnable()
    {
        if (eventSO != null)
        {
            eventSO.OnEventRaised += ResponseEvent;
        }
    }
    private void OnDisable()
    {
        if (eventSO != null)
        {
            eventSO.OnEventRaised -= ResponseEvent;
        }
    }
    private void ResponseEvent(T value)
    {
        responseEvent.Invoke(value);
    }
}
