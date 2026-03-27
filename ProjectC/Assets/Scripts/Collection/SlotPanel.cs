using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotPanel : MonoBehaviour, IPointerDownHandler
{
    public Slot slotScript;
    private bool isEnter = false;
    private List<Vector2> slotPosition;

    public List<FilterJobButton> filterHeros;

    private async void Start()
    {
        slotPosition = new List<Vector2>();
        SetSlotPosition();
    }

    private async void OnEnable()
    {
        await Cysharp.Threading.Tasks.UniTask.WaitUntil(() => GameManager.isReadyGameManager);
        SettingSlotButton();
    }

    private void SettingSlotButton()
    {
        var dataManager = Locator<DataManager>.Get();
        
        int count = 0;
        foreach(var hero in filterHeros)
        {
            if (dataManager.HasJob(hero.JobFilter))
            {
                hero.gameObject.SetActive(true);
                hero.rectTransform.anchoredPosition = slotPosition[count];
                count++;

                if(count >= slotPosition.Count)
                    count = slotPosition.Count - 1;
            }
            else
                hero.gameObject.SetActive(false);
        }
    }

    // 왜 Layout으로 안하고 직접 관리하는 이유는?
    // Grid Layout Group이나 지그재그로 Layoutgruop으로 관리하기 어렵기 떄문에 미리 Position을 저장해뒀다가 사용한다.
    private void SetSlotPosition()
    {
        slotPosition.Clear();

        float startX = 100f;
        float topY = -52f;
        float bottomY = -145.5f;
        float spacingX = 96f;
        float zigzagOffset = 46.25f;
        
        for(int i = 0; i < 6; i++)
        {
            slotPosition.Add(new Vector2(startX + (i * spacingX), topY));
        }

        for(int i = 0; i < 5; i++)
        {
            slotPosition.Add(new Vector2(startX + zigzagOffset + (i * spacingX), bottomY));
        }
    }


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

// 어떻게 해야할 지 생각해야 해보자.
// 