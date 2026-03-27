using UnityEngine;

public class FilterJobButton : MonoBehaviour
{
    public string JobFilter;
    public RectTransform rectTransform;

    private void Start()
    {
        rectTransform = this.GetComponent<RectTransform>();
    }

    public void SelectJob()
    {
        var eventManager = Locator<EventManager>.Get();
        string[] heros = { JobFilter };
        FilterParameter parameter = new FilterParameter(FilterType.Jump, _job: heros);
        eventManager.Notify(ChannelInfo.Filter, parameter);
    }


}
