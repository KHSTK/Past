using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombeButton : MonoBehaviour
{

    public Button[] optionButtons; // 按钮数组
    public RectTransform contentParent; // 内容生成的父物体
    public GameObject[] contentPrefabs; // 对应选项的内容预制体数组
    public GameObject contentPrefabs2;

    private GameObject currentContent; // 当前显示的内容
    private GameObject currentContent2;

    private void Start()
    {
        // 检查 contentParent 和 contentPrefabs 是否为空
        if (contentParent == null)
        {
            Debug.LogError("contentParent is not assigned!");
            return;
        }

        if (contentPrefabs == null || contentPrefabs.Length == 0)
        {
            Debug.LogError("contentPrefabs is not assigned or empty!");
            return;
        }

        // 为每个按钮绑定点击事件
        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (optionButtons[i] == null)
            {
                Debug.LogError($"Button at index {i} is not assigned!");
                continue;
            }

            int index = i;
            optionButtons[i].onClick.AddListener(() => OnOptionSelected(index));
        }
    }

    private void OnOptionSelected(int index)
    {
        // 检查索引是否越界
        if (index < 0 || index >= contentPrefabs.Length)
        {
            Debug.LogError($"Index {index} is out of range for contentPrefabs!");
            return;
        }

        // 销毁之前的内容
        if (currentContent != null)
        {
            Destroy(currentContent);
            Destroy(currentContent2);
        }

        // 根据选中的按钮生成对应的内容
        currentContent = Instantiate(contentPrefabs[index], contentParent);
        currentContent2 = Instantiate(contentPrefabs2, optionButtons[index].transform);
        currentContent.transform.localPosition = Vector3.zero; // 设置内容的位置
    }
}


