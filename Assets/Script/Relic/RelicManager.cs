using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RelicManager : MonoBehaviour
{
    public List<BaseRelic> activeRelics = new();
    public RelicConfigSO relicConfig;
    public GameObject relicPrefab;
    public GameObject relicPanel;
    public GameObject relicShopPanel;
    public GameObject relicGuidePanel;
    public Dictionary<BaseRelic, RelicEntity> activeRelicMap = new();
    public void OnBeforeDamage(DamageContext ctx)
    {
        StartCoroutine(ApplyRelicsRoutine(ctx));
    }
    private IEnumerator ApplyRelicsRoutine(DamageContext ctx)
    {
        foreach (var relic in activeRelics.Where(r => !r.isDamageRelic))
        {
            relic.ApplyEffect(ctx);
            yield return new WaitForSeconds(0.3f); // 保留分步效果
        }
    }
    public void AddRelic(BaseRelic newRelic)
    {
        activeRelics.Add(newRelic);
        CreateRelicEntity(relicShopPanel.transform, newRelic);
        RelicEntity entity = CreateRelicEntity(relicPanel.transform, newRelic);
        activeRelicMap.Add(newRelic, entity);
    }
    public RelicEntity CreateRelicEntity(Transform parent, BaseRelic relic)
    {
        var relicEntity = Instantiate(relicPrefab, parent.transform).GetComponent<RelicEntity>();
        relicEntity.Init(relicConfig.relics.Find(r => r.relicEffect == relic));
        return relicEntity;
    }
    // 获取藏品世界坐标
    public Vector3 GetRelicWorldPosition(BaseRelic relic)
    {
        if (activeRelicMap.TryGetValue(relic, out RelicEntity entity))
        {
            // 使用UI坐标转换
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                entity.GetComponent<RectTransform>(),
                entity.transform.position,
                Camera.main,
                out Vector3 worldPos
            );
            return worldPos + new Vector3(-2f, 0, 1f); // 添加偏移
        }
        return Vector3.zero;
    }
    public void ResetRelics()
    {
        activeRelics.Clear();
        activeRelicMap.Clear();
    }
}
