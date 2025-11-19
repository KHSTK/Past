using UnityEngine;
using DG.Tweening;
public class MenuExist : MonoBehaviour
{
    public void OnPointerClick()
    {
        Debug.Log("退出游戏");
        Application.Quit();
    }
}
