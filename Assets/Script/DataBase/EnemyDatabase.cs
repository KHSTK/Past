using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Database/EnemyDataBase")]
public class EnemyDatabase : ScriptableObject
{
    public List<EnemyVisualData> enemyVisualData = new List<EnemyVisualData>();
    [System.Serializable]
    public class EnemyVisualData
    {
        public string enemyId;
        public Sprite enemySprite;
        public RuntimeAnimatorController enemyAnimator;
    }
    public Sprite GetEnemySprite(string enemyId)
    {
        Debug.Log(enemyId);
        Debug.Log(enemyVisualData.Find(x => x.enemyId == enemyId).enemySprite);
        return enemyVisualData.Find(x => x.enemyId == enemyId).enemySprite;
    }
    public RuntimeAnimatorController GetEnemyAnimatorController(string enemyId)
    {
        return enemyVisualData.Find(x => x.enemyId == enemyId).enemyAnimator;
    }
}
