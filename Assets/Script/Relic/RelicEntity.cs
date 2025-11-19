using UnityEngine;
using UnityEngine.UI;

public class RelicEntity : MonoBehaviour
{
    [SerializeField] private Image relicImage;
    [SerializeField] private StringEventSO RelicDescriptionEvent;
    private RelicData currentRelic;
    public void Init(RelicData relicData)
    {
        currentRelic = relicData;
        Debug.Log("Relic Initialized" + currentRelic.relicName);
        relicImage.sprite = relicData.relicIcon;
    }
    public void ShowRelicDescription()
    {
        RelicDescriptionEvent.RaiseEvent(currentRelic.relicName + ":" + currentRelic.relicDescription, this);
        Debug.Log("Relic Description Shown" + currentRelic.relicDescription);
    }
    public void HideRelicDescription()
    {
        RelicDescriptionEvent.RaiseEvent("0", this);
        Debug.Log("Relic Description Hidden");
    }
}
