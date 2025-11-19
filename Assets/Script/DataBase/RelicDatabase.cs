using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Database/RelicDatabase")]
public class RelicDatabase : ScriptableObject
{
    public List<RelicVisualData> relicVisualData = new List<RelicVisualData>();
    [System.Serializable]
    public class RelicVisualData
    {
        public string relicID;
        public BaseRelic Relic;
    }
    public BaseRelic GetRelicVisualData(string relicId)
    {
        return relicVisualData.Find(x => x.relicID == relicId).Relic;
    }

}
