using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Config/EnemyConfig")]
public class EnemyConfigSO : ScriptableObject
{
    public List<EnemyData> enemyDataList = new List<EnemyData>();
    /// <summary>
    /// 根据类型获取随机敌人数据
    /// </summary>
    /// <param name="targetType">敌人类型</param>
    /// <returns></returns>
    public EnemyData GetRandomEnemyDataByType(EnemyType targetType)
    {
        var candidates = enemyDataList.FindAll(x => x.enemyType == targetType);
        return candidates.Count > 0 ? candidates[Random.Range(0, candidates.Count)] : null;
    }
}
[System.Serializable]
public class EnemyData
{
    public string enemyID;
    public string enemyName;
    public string enemyDescription;
    public int maxHp;
    public int damage;
    public int time;
    public EnemyType enemyType;
    public EnemyEffect enemyEffect;
}

