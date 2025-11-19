using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextControl : MonoBehaviour
{
    //fade速度
    public float speed = 1f;
    Color color;

    void Start()
    {
        color = GetComponent<TextMeshPro>().color;  //获取当前游戏对象的Text组件的颜色信息，并将其赋值给color变量
    }
    void OnEnable()
    {
        color.a = 1f;  //将color变量的透明度分量设置为1，表示完全不透明
    }
    void Update()
    {
        if (gameObject.activeSelf)  //如果当前游戏对象处于激活状态，并且isFade为true
        {
            color.a -= Time.deltaTime * speed;  //将color变量的透明度分量减去一定值，实现渐变效果
            GetComponent<TextMeshPro>().color = color;  //将更新后的color变量赋值给当前游戏对象的Text组件的颜色信息
        }
        if (color.a <= 0)
        {
            gameObject.SetActive(false);
        }  //如果color变量的透明度分量小于等于0

    }
    void OnDisable()
    {
        color.a = 1f;
    }
}
