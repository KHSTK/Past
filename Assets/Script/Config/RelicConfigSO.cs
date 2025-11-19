using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Config/RelicConfigSO")]

public class RelicConfigSO : ScriptableObject
{
    public List<RelicData> relics = new List<RelicData>();
    public BaseRelic GetRelicVisualData(string relicId)
    {
        return relics.Find(x => x.relicID == relicId).relicEffect;
    }
}
[System.Serializable]
public class RelicData
{
    public string relicID;
    public string relicName;
    [TextArea] public string relicDescription;
    public int price;
    public Sprite relicIcon;
    public BaseRelic relicEffect;
}

