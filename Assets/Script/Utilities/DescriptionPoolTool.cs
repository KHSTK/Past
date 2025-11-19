using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

public class DescriptionPoolTool : MonoBehaviour
{
    public Canvas uiCanvas;
    public GameObject objPrefab;
    [HideInInspector] public ObjectPool<GameObject> pool;
    private Vector2 mouseCanvasPos;
    public List<GameObject> poolObjectList = new List<GameObject>();

    private void Awake()
    {
        pool = new ObjectPool<GameObject>(
            createFunc: CreatePooledObject,
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) =>
            {
                poolObjectList.Remove(obj);
                Destroy(obj);
            },
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 20
        );
        PreFillPool(10);
    }
    private GameObject CreatePooledObject()
    {
        var obj = Instantiate(objPrefab, transform);
        poolObjectList.Add(obj);
        return obj;
    }

    private void PreFillPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = pool.Get();
            pool.Release(obj);
        }
    }
    //获取对象
    public GameObject GetGameObjectFromPool()
    {
        return pool.Get();
    }
    //释放对象
    public void ReleaseGameObjectToPool(GameObject obj)
    {
        pool.Release(obj);
    }
    //销毁所有对象池子物体
    [ContextMenu("刷新对象池")]
    public void DestroyAllObjects()
    {
        foreach (var obj in poolObjectList)
        {
            Destroy(obj);
        }
        pool.Clear();
        poolObjectList.Clear();
        PreFillPool(10);
    }
    private void GetCanvasPosition(Vector3 screenPosition)
    {
        Vector2 canvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            uiCanvas.GetComponent<RectTransform>(),
            screenPosition,
            uiCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : uiCanvas.worldCamera,
            out canvasPos
        );
        mouseCanvasPos = canvasPos;
    }
    public void CreatDescription(string value)
    {
        GetCanvasPosition(Input.mousePosition);
        var obj = GetGameObjectFromPool();
        obj.transform.localPosition = mouseCanvasPos + new Vector2(+280, +70);
        obj.GetComponentInChildren<TextMeshProUGUI>().text = value;
        if (value == "0")
        {
            DestroyAllObjects();
        }
    }
}
