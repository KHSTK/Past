using UnityEngine;

public class MenuEntry : MonoBehaviour
{
    [Header("广播")]
    public ObjectEventSO newGameEvent;
    public void OnPointerClick()
    {
        Debug.Log("进入游戏");
        newGameEvent.RaiseEvent(this, this);
    }
}
