using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class MinionCard : IMinion, IChannel
{
    private GameObject spawnObject;
    private BattleFieldObjectInformation battleInfo;
    private List<ITargetable> targetObjects;

    private List<CardAbilityData> abilitys;


    public async UniTask Execute(BattleFieldObjectInformation battleCard)
    {
        Debug.Log("Execute Minion");
        battleInfo = battleCard;

        var resourceManager = Locator<ResourceManager>.Get();
        var battleManager = Locator<BattleManager>.Get();

        var battleComponent = battleCard.card.GetComponent<BattleCard>();
        var battleCardResource = battleComponent.GetResourceData();

        // 어떻게 Struct 처리로 value로 바꿔서 처리했다.
        MinionResourceData minionResource = new MinionResourceData();
        minionResource.cardImage = battleCardResource.cardImage.sprite;
        minionResource.legand = battleCardResource.legandPortrait.activeSelf;
        minionResource.attack = battleCardResource.attack.text;
        minionResource.health = battleCardResource.health.text;

        // 하수인 뼈대 생성
        var card = await resourceManager.Get<GameObject>("Card Entity");
        var battleField = battleManager.GetBattleField();
        var battleFieldComponent = battleField.GetComponent<BattleField>();

        var cardEntity = card.GetComponent<CardEntity>();
        cardEntity.CardResourceSetting(minionResource);

        cardEntity.card = battleComponent;

        if (battleCard.isPlayer)
            spawnObject = GameObject.Instantiate(card, battleFieldComponent.playerField.transform);
        else
            spawnObject = GameObject.Instantiate(card, battleFieldComponent.enemyField.transform);

        // 몇 번째 index인지도 알아야한다.
        spawnObject.transform.SetSiblingIndex(battleCard.battleIndex);

        // 여기서 하수인이 부드럽게 나오는 연출을 해야할 수도 있다.
        // 이건 어떻게 동작하는지 한번 봐야한다.
        // 카드에서도 생겼던 문제다.


        // 카드 능력들을 확인해서 생성이 되는지 아닌지 확인한다.
        var abilityData = battleComponent.GetAbilityData();
        abilitys = abilityData.GetCardAbilityDatas();

        // 여기서 적인지 아군인지 알 수 있지 않나?
        var newCardEntityComponent = spawnObject.GetComponent<CardEntity>();
        //newCardEntityComponent.EntitySetting();
        newCardEntityComponent.EntitySetting(battleCard.isPlayer, abilitys);

        // 비어 있으면 넘긴다.
        if (abilitys == null)
            return;

        // 돌면서 타겟팅이 가능한 전투의함성이 있는지 확인한다.
        bool isTargetExistBattleCry = false;
        foreach (var ability in abilitys)
        {
            if (ability.isTargetting)
            {
                // Object가 생성된 다음 프레임에 Position이 잘 작동 되서 한 프레임을 기다린다.
                await UniTask.Yield();
                EnableTargetingObject(spawnObject, ability.target, battleFieldComponent);
                // 여기서 데미지 처리를 위한 무언가?
                isTargetExistBattleCry = true;
                break;
            }
        }

        // 일단 무조건 생성한다.
        // 하수인 덩어리를 무조건 생성하고 Target이 필요한 경우 추가 작업을 하는 것 같다?
        if (isTargetExistBattleCry)
        {
            // Target UI가 생성된다.
            var targetArrow = battleManager.GetTargetPanel();
            var selectingArrowComponent = targetArrow.GetComponent<SelectingArrow>();

            var eventManager = Locator<EventManager>.Get();
            eventManager.Subscription(ChannelInfo.TargetSelected, HandleEvent);
            eventManager.Subscription(ChannelInfo.TargetCanceled, HandleEvent);

            var spawnRect = spawnObject.GetComponent<RectTransform>();
            selectingArrowComponent.LineSetting(spawnRect.position);
        }
        else
        {
            // 능력들을 처리하면 안 될 것 같은데?
            var executor = new AbilityExecutor();
            var selfEntity = spawnObject.GetComponent<IEntity>();
            // 여기서 None이 아닌 실제 능력을 넣어야 한다.
            // 근데 여기서 하게 되면 attacked와 death도 적용된다.
            executor.Execute(abilitys, AbilityTrigger.None, self: selfEntity);
            executor.Execute(abilitys, AbilityTrigger.OnMinionSummon, self: selfEntity);
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

                // 근데 반드시 데미지 처리가 이뤄지는 것은 아니야.
                // 힐을 할 수도 있고 데미지를 줄 수도 있고 체력을 늘릴 수도 있어.
                // information 안에는 선택된 하수인이 들어가 있어.

                // abilitys에는 여러가지가 있다. 
                // 이걸 처리하면 되겠지?
                if(information is IEntity targetEntity)
                {
                    var executor = new AbilityExecutor();
                    var selfEntity = spawnObject.GetComponent<IEntity>();
                    executor.Execute(abilitys, AbilityTrigger.Battlecry, self: selfEntity, target: targetEntity);
                }

                break;
            case ChannelInfo.TargetCanceled:
                eventManager.Unsubscription(ChannelInfo.TargetSelected, HandleEvent);
                eventManager.Unsubscription(ChannelInfo.TargetCanceled, HandleEvent);

                // 하수인을 삭제하고 카드를 생성해야 한다.
                GameObject.Destroy(spawnObject);
                spawnObject = null;

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


    // 자기 자신을 제외한 Target을 하려면 어떻게 해야할까?
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
                targetObjects = battlefield.EnemyField<ITargetable>(Itargetable);
                break;
            case AbilityTarget.AllEnemyMinionsExceptTarget:
                // 적 하수인만 해당
                targetObjects = battlefield.EnemyField<ITargetable>(Itargetable);
                break;
            case AbilityTarget.AllFriendlyMinionsExceptTarget:
                // 아군 하수인만 해당
                targetObjects = battlefield.PlayerField<ITargetable>(Itargetable);
                break;
            case AbilityTarget.TargetFriendlyMinion:
                // 아군 하수인만 해당
                targetObjects = battlefield.PlayerField<ITargetable>(Itargetable);
                break;
            case AbilityTarget.AllMinionsExceptTarget:
                // 모든 하수인만 해당
                targetObjects = battlefield.AllField<ITargetable>(Itargetable);
                break;
            case AbilityTarget.AllTarget:
                // 모든 하수인 + 영웅 해당
                targetObjects = battlefield.AllField<ITargetable>(Itargetable);
                //targetObjects.Add(battlefield.enemyHero);
                //targetObjects.Add(battlefield.playerHero);
                break;
            case AbilityTarget.AllTargetMinion:
                // 모든 하수인만 해당
                targetObjects = battlefield.AllField<ITargetable>(Itargetable);
                break;
        }

        Debug.Log(targetObjects.Count);

        foreach (var targetInterface in targetObjects)
        {
            targetInterface.OnTargeted();
        }
    }
}

public struct MinionResourceData
{
    public Sprite cardImage;
    public bool legand;
    public string attack;
    public string health;
}