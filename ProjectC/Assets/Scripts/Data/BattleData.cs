using UnityEngine;

public enum AbilityTrigger
{
    None,
    Battlecry,
    Deathrattle,
    OnTurnEnd,
    OnTurnStart,
    OnMinionSummon,
    OnSpellCast,
    OnAttacking,
    OnAttacked,
}

public enum AbilityAction
{
    None,
    Damage,
    Heal,
    Taunt,
    Shield,
    Token,
    DrawCard,
    Attack,
    Health,
    Remove,
    Charge,
    Stealth,
    Freeze,
    Windfury,
}

public enum AbilityTarget
{
    TargetFriendlyMinion,           // 내 하수인 타겟
    TargetEnemyMinion,              // 적 하수인 타겟
    AllTargetMinion,                // 모든 하수인 타겟
    AllTarget,                      // 모든 하수인 + 영웅 타겟
    AllEnemyTarget,                 // 모든 적 하수인 + 영웅 타겟
    AllFrendlyTarget,               // 모든 아군 하수인 + 영웅 타겟
    AllEnemyTargetExceptSelf,       
    AllFrendlyTargetExceptSelf,

    AllFriendlyMinions,             // 아군 하수인들
    AllEnemyMinions,                // 적 하수인들
    AllMinions,                     // 모든 하수인들

    RandomFriendlyMinion,           // 랜덤 아군 하수인
    RandomEnemyMinion,              // 랜덤 적 하수인
    AllRandomMinion,                // 랜덤 모든 하수인

    FriendlyHero,                   // 내 영웅
    EnemyHero,                      // 적 영웅
    AllHeroes,                      // 내 + 적 영웅

    AllFriendlyMinionsExceptSelf,   // 자기 자신 제외 아군 하수인
    AllMinionsExceptSelf,           // 자기 자신 제외 모든 하수인

    AllFriendlyMinionsExceptTarget, // 타겟 제외 모든 아군 하수인
    AllEnemyMinionsExceptTarget,    // 타겟 제외 모든 적 하수인
    AllMinionsExceptTarget,         // 타겟 제외 모든 하수인

    AdjacentMinions,                // 인접한 하수인

    FriendlyHand,                   // 내 핸드
    EnemyHand,                      // 적 핸드

    FriendlyDeck,                   // 내 덱
    EnemyDeck,                      // 적 덱

    Self,                           // 자기 자신
}

public enum AbilityCondition
{
    None,
    Over,
    Under,
    MinionType,
    SpellType,
    HasWeapon,
}

public enum AbilityConditionStat
{
    None,
    Attack,
    Health,
    Cost,
}

public struct CardAbilityData
{
    public bool isExistAbility;
    public AbilityTrigger trigger;
    public AbilityAction action;
    public AbilityTarget target;
    public int value;
    public AbilityCondition condition;
    public AbilityConditionStat conditionState;
    public int conditionValue;
    public string conditionType;
    public bool isTargetting;
    public bool isTempory;
}
