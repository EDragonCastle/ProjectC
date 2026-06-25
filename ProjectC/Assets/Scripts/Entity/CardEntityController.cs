using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CardEntityController : MonoBehaviour, IChannel, EntityController
{
    public CardEntity entity;

    private List<ITargetable> targets;

    public MonoBehaviour GetEntity() => entity;


    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Unsubscription(ChannelInfo.TargetSelected, HandleEvent);
        eventManager.Unsubscription(ChannelInfo.TargetCanceled, HandleEvent);

        foreach (var target in targets)
        {
            target.OnUnTargeted();
        }
        targets.Clear();

        switch (channel)
        {
            case ChannelInfo.TargetSelected:
                // 여기서는 Damage 처리를 해야 한다.
                if(information is ICombatable combat)
                {
                    var clash = new Clash();
                    clash.Execute(entity, combat);
                    entity.OnAttacking();
                    entity.SetAttacked();
                }

                break;
            case ChannelInfo.TargetCanceled:
                break;
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        // UI가 띄우는데 바로 뜨는게 아니라 몇 초 후에 뜬다.

        // 공격 중이 아닐 때는 몇 초 후에 해당 card의 정보가 나오는데 

        // 공격 중일 때는 내가 죽는 지 안 죽는 지 보인다.
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        // UI 설명 모습이 보이지 않는다.
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Drag 형식이 활성화 된다.
        if (!entity.isTargetable) return;
        entity.OnPointerDown();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 공격 유무를 따지겠지?
        ActiveSelectingArrow();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 공격 유무를 따지겠지?
        ActiveSelectingArrow();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("드래그 중이다.");

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("드래그 끝났다.");
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach(var result in results)
        {
            // Controller에 닿는다.
            // 여기서 EntityController을 출력했다. EntityController는 하수인 Controller도 해당되고 영웅 Controller를 해당하기도 한다. 
            var entityController = result.gameObject.GetComponent<EntityController>();

            // EntityController에 인식이 되고 있다.
            if(entityController != null)
            {
                var combatable = entityController.GetEntity() as ICombatable;
                if(combatable != null)
                {
                    var eventManager = Locator<EventManager>.Get();
                    eventManager.Notify(ChannelInfo.TargetSelected, combatable);
                    return;
                }
            }
        }
    }

    // 드래그 3종 interface이 있어야 BeginDrag, EndDrag, OnDrag가 잘 먹히는 것 같다.
    private void ActiveSelectingArrow()
    {
        if (!entity.CanAttack()) return;

        var battleManager = Locator<BattleManager>.Get();
        var targetArrow = battleManager.GetTargetPanel();
        var selectingArrowComponent = targetArrow.GetComponent<SelectingArrow>();

        var entityRect = this.GetComponent<RectTransform>();
        selectingArrowComponent.LineSetting(entityRect.position);

        var eventManager = Locator<EventManager>.Get();
        eventManager.Subscription(ChannelInfo.TargetSelected, HandleEvent);
        eventManager.Subscription(ChannelInfo.TargetCanceled, HandleEvent);

        var battleField = battleManager.GetBattleField().GetComponent<BattleField>();
        targets = battleField.EnemyFieldChildren();

        foreach(var target in targets)
        {
            target.OnTargeted();
        }
    }
}
