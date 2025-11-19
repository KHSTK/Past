using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ClickPoolTool : MonoBehaviour
{
    public Canvas uiCanvas;
    public GameObject objPrefab;
    [HideInInspector] public ObjectPool<GameObject> pool;

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
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 effectPosition = GetCanvasPosition(Input.mousePosition);
            SpawnEffectAtPosition(effectPosition);
        }
    }
    private Vector2 GetCanvasPosition(Vector3 screenPosition)
    {
        Vector2 canvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            uiCanvas.GetComponent<RectTransform>(),
            screenPosition,
            uiCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : uiCanvas.worldCamera,
            out canvasPos
        );
        return canvasPos;
    }

    // 在指定位置生成特效
    private void SpawnEffectAtPosition(Vector2 position)
    {
        if (pool.CountInactive == 0 && poolObjectList.Count >= 20)
        {
            // 池满时不创建新特效
            return;
        }

        GameObject effect = GetGameObjectFromPool();
        RectTransform rect = effect.GetComponent<RectTransform>();
        rect.localPosition = position;

        // 重置状态
        effect.SetActive(true);

        // 启动协程在动画结束后回收
        StartCoroutine(ReturnEffectAfterAnimation(effect));
    }
    // 动画结束后回收对象
    private System.Collections.IEnumerator ReturnEffectAfterAnimation(GameObject effect)
    {
        yield return new WaitUntil(() => effect.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime % 1.0f > 0.95f
            && effect.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Click"));

        // 确保对象未被提前回收
        if (effect != null && effect.activeSelf)
        {
            ReleaseGameObjectToPool(effect);
        }
    }
}
