using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;


public class SpellCard : ISpell, IChannel
{
    private BattleFieldObjectInformation battleInfo;
    private List<ITargetable> targetObjects;


    public async UniTask Execute(BattleFieldObjectInformation battleCard)
    {
        Debug.Log("Execute Spell");
        battleInfo = battleCard;

        // 주문은 뭐 하수인을 만들 필요가 없긴하지.

        // Target인지 아닌지 먼저 확인해야 한다.
        var battleComponent = battleCard.card.GetComponent<BattleCard>();
        var abilityData = battleComponent.GetAbilityData();
        var cardAbilityDatas = abilityData.GetCardAbilityDatas();

        var battleManager = Locator<BattleManager>.Get();
        var battleField = battleManager.GetBattleField();
        var battleFieldComponent = battleField.GetComponent<BattleField>();

        bool isTargetting = false;
        foreach(var ability in cardAbilityDatas)
        {
            if(ability.isTargetting)
            {
                await UniTask.Yield();
                // 일단은 놔둬볼까
                EnableTargetingObject(battleCard.card, ability.target, battleFieldComponent);
                isTargetting = true;
                break;
            }
        }

        if(isTargetting)
        {
            var targetArrow = battleManager.GetTargetPanel();
            var selectingArrowComponent = targetArrow.GetComponent<SelectingArrow>();

            var eventManager = Locator<EventManager>.Get();
            eventManager.Subscription(ChannelInfo.TargetSelected, HandleEvent);
            eventManager.Subscription(ChannelInfo.TargetCanceled, HandleEvent);

            // 영웅 Rect 위치를 받아서 넣어야 한다.
            selectingArrowComponent.LineSetting(new Vector3(0, 0, 0));
        }
        else
        {
            // Effect 처리
        }
    }

    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        var eventManager = Locator<EventManager>.Get();

        switch (channel)
        {
            case ChannelInfo.TargetSelected:
                eventManager.Unsubscription(ChannelInfo.TargetSelected, HandleEvent);
                eventManager.Unsubscription(ChannelInfo.TargetCanceled, HandleEvent);
                foreach (var target in targetObjects)
                {
                    target.OnUnTargeted();
                }

                // Spell Effect 처리를 해야 한다.
                
                break;
            case ChannelInfo.TargetCanceled:
                eventManager.Unsubscription(ChannelInfo.TargetSelected, HandleEvent);
                eventManager.Unsubscription(ChannelInfo.TargetCanceled, HandleEvent);

                // Hand로 다시 카드를 되돌려야 한다.
                var battleManager = Locator<BattleManager>.Get();
                var hand = battleManager.GetHandParent();
                var handComponent = hand.GetComponent<Hand>();

                // 우리는 정보만 넘겨주자
                // 근데 하수인이 취소된 위치에 있어야 하지 않을까?
                handComponent.InsertCard(battleInfo).Forget();

                foreach (var target in targetObjects)
                {
                    target.OnUnTargeted();
                }
                break;
        }
    }


    private void EnableTargetingObject(GameObject card, AbilityTarget target, BattleField battlefield)
    {
        var Itargetable = card.GetComponent<ITargetable>();

        // 근데 영웅은 CardEntity가 아닌데
        // 그러면 TargetableObject Script를 넣으면 해결되긴하는데
        // 그거보다 Interface로 하는 게 더 낫다고 판단.
        targetObjects = new List<ITargetable>();

        // 여기서 List를 추가해야 하나?
        switch (target)
        {
            case AbilityTarget.TargetEnemyMinion:
                // 적 하수인만 해당
                targetObjects = battlefield.EnemyFieldChildren(Itargetable);
                break;
            case AbilityTarget.AllEnemyMinionsExceptTarget:
                // 적 하수인만 해당
                targetObjects = battlefield.EnemyFieldChildren(Itargetable);
                break;
            case AbilityTarget.AllFriendlyMinionsExceptTarget:
                // 아군 하수인만 해당
                targetObjects = battlefield.PlayerFieldChildren(Itargetable);
                break;
            case AbilityTarget.TargetFriendlyMinion:
                // 아군 하수인만 해당
                targetObjects = battlefield.PlayerFieldChildren(Itargetable);
                break;
            case AbilityTarget.AllMinionsExceptTarget:
                // 모든 하수인만 해당
                targetObjects = battlefield.AllFieldChildren(Itargetable);
                break;
            case AbilityTarget.AllTarget:
                // 모든 하수인 + 영웅 해당
                targetObjects = battlefield.AllFieldChildren(Itargetable);
                //targetObjects.Add(battlefield.enemyHero);
                //targetObjects.Add(battlefield.playerHero);
                break;
            case AbilityTarget.AllTargetMinion:
                // 모든 하수인만 해당
                targetObjects = battlefield.AllFieldChildren(Itargetable);
                break;
        }

        Debug.Log(targetObjects.Count);

        foreach (var targetInterface in targetObjects)
        {
            targetInterface.OnTargeted();
        }
    }
}
