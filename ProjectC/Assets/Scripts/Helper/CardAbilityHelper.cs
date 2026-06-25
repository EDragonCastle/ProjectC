using System.Collections.Generic;

public class CardAbilityHelper
{
    public List<CardAbilityData> CardAbilitySetting(uint cardId)
    {
        var dataManager = Locator<DataManager>.Get();
        var abilityTable = dataManager.GetAbilityData();

        List<CardAbilityData> cardAbilityDatas = new List<CardAbilityData>();

        if(!abilityTable.ContainsKey(cardId)) {
            return cardAbilityDatas;
        }

        var abilityDatas = abilityTable[cardId];

        for(int i = 0; i < abilityDatas.Count; i++)
        {
            var originAbilityData = abilityDatas[i];

            CardAbilityData abilityData = new CardAbilityData();
            abilityData.isExistAbility = true;
            abilityData.trigger = StringToAbilityTrigger(originAbilityData.actionTrigger);
            abilityData.action = StringToEffectAction(originAbilityData.action);
            abilityData.target = StringToAbilityTarget(originAbilityData.target);
            abilityData.value = originAbilityData.value;
            abilityData.condition = StringToAbilityCondition(originAbilityData.condition);
            abilityData.conditionState = StringToConditionStat(originAbilityData.conditionStat);
            abilityData.conditionType = originAbilityData.conditionMinionType;
            abilityData.conditionValue = originAbilityData.conditionValue;
            abilityData.isTargetting = originAbilityData.isTargetting;
            cardAbilityDatas.Add(abilityData);
        }

        return cardAbilityDatas;
    }

    public string[] CardTypes(uint UID)
    {
        var dataManager = Locator<DataManager>.Get();
        var cardTable = dataManager.GetCardData();

        if (!cardTable.ContainsKey(UID)) {
            return null;
        }

        var cardData = cardTable[UID];
        return cardData.cardTypes;
    }


    private AbilityTrigger StringToAbilityTrigger(string trigger)
    {
        AbilityTrigger abilityTrigger = AbilityTrigger.None;

        switch(trigger)
        {
            case "Battlecry":
                abilityTrigger = AbilityTrigger.Battlecry;
                break;
            case "Deathrattle":
                abilityTrigger = AbilityTrigger.Deathrattle;
                break;
            case "OnTurnEnd":
                abilityTrigger = AbilityTrigger.OnTurnEnd;
                break;
            case "OnTurnStart":
                abilityTrigger = AbilityTrigger.OnTurnStart;
                break;
            case "OnMinionSummon":
                abilityTrigger = AbilityTrigger.OnMinionSummon;
                break;
            case "OnSpellCast":
                abilityTrigger = AbilityTrigger.OnSpellCast;
                break;
            case "OnAttacking":
                abilityTrigger = AbilityTrigger.OnAttacking;
                break;
            case "OnAttacked":
                abilityTrigger = AbilityTrigger.OnAttacked;
                break;
            default:
                abilityTrigger = AbilityTrigger.None;
                break;
        }

        return abilityTrigger;
    }

    
    private AbilityAction StringToEffectAction(string action)
    {
        AbilityAction effectAction = AbilityAction.None;

        switch(action)
        {
            case "Damage":
                effectAction = AbilityAction.Damage;
                break;
            case "Heal":
                effectAction = AbilityAction.Heal;
                break;
            case "Taunt":
                effectAction = AbilityAction.Taunt;
                break;
            case "Shield":
                effectAction = AbilityAction.Shield;
                break;
            case "Token":
                effectAction = AbilityAction.Token;
                break;
            case "DrawCard":
                effectAction = AbilityAction.DrawCard;
                break;
            case "Attack":
                effectAction = AbilityAction.Attack;
                break;
            case "Health":
                effectAction = AbilityAction.Health;
                break;
            case "Remove":
                effectAction = AbilityAction.Remove;
                break;
            case "Charge":
                effectAction = AbilityAction.Charge;
                break;
            case "Stealth":
                effectAction = AbilityAction.Stealth;
                break;
            case "Freeze":
                effectAction = AbilityAction.Freeze;
                break;
            case "Windfury":
                effectAction = AbilityAction.Windfury;
                break;
        }

        return effectAction;
    }

    private AbilityTarget StringToAbilityTarget(string target)
    {
        AbilityTarget abilityTarget = AbilityTarget.Self;

        switch(target)
        {
            case "TargetFriendlyMinion":
                abilityTarget = AbilityTarget.TargetFriendlyMinion;
                break;
            case "TargetEnemyMinion":
                abilityTarget = AbilityTarget.TargetEnemyMinion;
                break;
            case "AllTargetMinion":
                abilityTarget = AbilityTarget.AllTargetMinion;
                break;
            case "AllTarget":
                abilityTarget = AbilityTarget.AllTarget;
                break;
            case "AllFriendlyMinions":
                abilityTarget = AbilityTarget.AllFriendlyMinions;
                break;
            case "AllEnemyMinions":
                abilityTarget = AbilityTarget.AllEnemyMinions;
                break;
            case "AllMinions":
                abilityTarget = AbilityTarget.AllMinions;
                break;
            case "RandomFriendlyMinion":
                abilityTarget = AbilityTarget.RandomFriendlyMinion;
                break;
            case "RandomEnemyMinion":
                abilityTarget = AbilityTarget.RandomEnemyMinion;
                break;
            case "AllRandomMinion":
                abilityTarget = AbilityTarget.AllRandomMinion;
                break;
            case "FriendlyHero":
                abilityTarget = AbilityTarget.FriendlyHero;
                break;
            case "EnemyHero":
                abilityTarget = AbilityTarget.EnemyHero;
                break;
            case "AllHeroes":
                abilityTarget = AbilityTarget.AllHeroes;
                break;
            case "AllFriendlyMinionsExceptSelf":
                abilityTarget = AbilityTarget.AllFriendlyMinionsExceptSelf;
                break;
            case "AllMinionsExceptSelf":
                abilityTarget = AbilityTarget.AllMinionsExceptSelf;
                break;
            case "AllFriendlyMinionsExceptTarget":
                abilityTarget = AbilityTarget.AllFriendlyMinionsExceptTarget;
                break;
            case "AllEnemyMinionsExceptTarget":
                abilityTarget = AbilityTarget.AllEnemyMinionsExceptTarget;
                break;
            case "AllMinionsExceptTarget":
                abilityTarget = AbilityTarget.AllMinionsExceptTarget;
                break;
            case "AdjacentMinions":
                abilityTarget = AbilityTarget.AdjacentMinions;
                break;
            case "FriendlyHand":
                abilityTarget = AbilityTarget.FriendlyHand;
                break;
            case "EnemyHand":
                abilityTarget = AbilityTarget.EnemyHand;
                break;
            case "FriendlyDeck":
                abilityTarget = AbilityTarget.FriendlyDeck;
                break;
            case "EnemyDeck":
                abilityTarget = AbilityTarget.EnemyDeck;
                break;
            default:
                abilityTarget = AbilityTarget.Self;
                break;
        }
        return abilityTarget;
    }

    private AbilityCondition StringToAbilityCondition(string condition)
    {
        AbilityCondition abilityCondition = AbilityCondition.None;

        switch(condition)
        {
            case "Over":
                abilityCondition = AbilityCondition.Over;
                break;
            case "Under":
                abilityCondition = AbilityCondition.Under;
                break;
            case "MinionType":
                abilityCondition = AbilityCondition.MinionType;
                break;
            case "SpellType":
                abilityCondition = AbilityCondition.SpellType;
                break;
            case "HasWeapon":
                abilityCondition = AbilityCondition.HasWeapon;
                break;
            default:
                abilityCondition = AbilityCondition.None;
                break;
        }

        return abilityCondition;
    }

    private AbilityConditionStat StringToConditionStat(string conditionStat)
    {
        AbilityConditionStat stat = AbilityConditionStat.None;
        switch(conditionStat)
        {
            case "Attack":
                stat = AbilityConditionStat.Attack;
                break;
            case "Health":
                stat = AbilityConditionStat.Health;
                break;
            case "Cost":
                stat = AbilityConditionStat.Cost;
                break;
        }

        return stat;
    }
}

// 근데 매번 이렇게 하면 귀찮아지는데
// 근데 switch를 사용하려면 const를 사용해야해서 할 수 없다.