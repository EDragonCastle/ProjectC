using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotPanel : MonoBehaviour, IPointerDownHandler
{
    public Slot slotScript;
    private bool isEnter = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isEnter)
            return;
        slotScript.Ending(isEnter);
    }

    public void SelectJob(string hero)
    {
        var eventManager = Locator<EventManager>.Get();
        string[] heros = { hero };
        FilterParameter parameter = new FilterParameter(FilterType.Jump, _job: heros);
        eventManager.Notify(ChannelInfo.Filter, parameter);
    }
}