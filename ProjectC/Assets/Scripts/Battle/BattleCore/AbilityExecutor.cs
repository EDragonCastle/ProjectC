using System.Collections.Generic;

using UnityEngine;

public class AbilityExecutor
{
    private List<IEntity> targets;
    private bool isBuffer = false;
    private bool isTempory = false;

    public void Execute(List<CardAbilityData> abilitys, AbilityTrigger trigger, IEntity self = null, IEntity target = null)
    {
        // Execute가 핸드에서 카드를 냈을 때 실행되는 건가?
        Vector2Int stat = new Vector2Int();
        isTempory = false;
        isBuffer = false;

        foreach(var ability in abilitys)
        {
            // 능력이 있는지 확인한다.
            if (!ability.isExistAbility) continue;

            // Trigger와 다르다면 넘긴다.
            if (ability.trigger != trigger) continue;

            // AbilityTarget 처리한다.
            ApplyTarget(ability, self: self, target: target);

            // 그 다음 Condition을 처리한다.
            ApplyCondition(ability);

            // 마지막으로 Action을 처리한다.
            ApplyAction(ability, ref stat);
        }

        if(isBuffer) {
            Buffer(stat);
        }
    }

    public void Execute(List<CardAbilityData> abilitys, IEntity self = null, IEntity target = null)
    {
        Vector2Int stat = new Vector2Int();
        isTempory = false;
        isBuffer = false;

        foreach (var ability in abilitys)
        {
            // 능력이 있는지 확인한다.
            if (!ability.isExistAbility) continue;

            // AbilityTarget 처리한다.
            ApplyTarget(ability, self: self, target: target);

            // 그 다음 Condition을 처리한다.
            ApplyCondition(ability);

            // 마지막으로 Action을 처리한다.
            ApplyAction(ability, ref stat);
        }

        if (isBuffer) {
            Buffer(stat);
        }
    }

    private void ApplyAction(CardAbilityData ability, ref Vector2Int stat)
    {
        switch (ability.action)
        {
            case AbilityAction.Damage:
                Debug.Log($"대상에게 {ability.value}만큼 데미지을 준다");
                Damage(ability.value);
                break;
            case AbilityAction.Heal:
                Debug.Log($"대상에게 {ability.value}만큼 힐을 준다");
                Heal(ability.value);
                break;
            case AbilityAction.Remove:
                Debug.Log("대상을 제거");
                Remove();
                break;
            case AbilityAction.DrawCard:
                Debug.Log("하수인을 낸 사람이 카드를 뽑는다");
                DrawCard();
                break;
            case AbilityAction.Taunt:
                Debug.Log("대상에게 도발 부여");
                isTempory = ability.isTempory;
                ExecuteKeyword(KeywordType.Taunt);
                break;
            case AbilityAction.Token:
                Debug.Log("대상에게 Token을 소환한다");
                Token();
                break;
            case AbilityAction.Shield:
                Debug.Log("대상에게 쉴드를 부여한다");
                isTempory = ability.isTempory;
                ExecuteKeyword(KeywordType.DivineShield);
                break;
            case AbilityAction.Attack:
                Debug.Log("대상에게 공격력을 부여한다");
                isBuffer = true;
                stat.x = ability.value;
                isTempory = ability.isTempory;
                break;
            case AbilityAction.Health:
                Debug.Log("대상에게 체력을 부여한다");
                isBuffer = true;
                stat.y = ability.value;
                isTempory = ability.isTempory;
                break;
            case AbilityAction.Charge:
                Debug.Log("대상에게 돌진을 부여한다");
                isTempory = ability.isTempory;
                ExecuteKeyword(KeywordType.Charge);
                break;
            case AbilityAction.Stealth:
                isTempory = ability.isTempory;
                ExecuteKeyword(KeywordType.Stealth);
                break;
            case AbilityAction.Windfury:
                isTempory = ability.isTempory;
                ExecuteKeyword(KeywordType.Windfury);
                break;
        }
    }

    private void ApplyCondition(CardAbilityData ability)
    {
        // 근데 여기서 어떤 것을 해야 하지?
        // 뭐가 필요할까?

        // targets에서 condition을 처리해야 한다.
        // 근데 Condition에서 이게 공격력인지 체력인지 코스트인지 알 수 없다.
        // 확인할 수 있는 방법이 있을까?

        switch (ability.condition)
        {
            case AbilityCondition.None:
                break;
            case AbilityCondition.Over:
                Over(ability);
                break;
            case AbilityCondition.Under:
                Under(ability);
                break;
            case AbilityCondition.MinionType:
                MinionType(ability);
                break;
            case AbilityCondition.SpellType:
                SpellType(ability);
                break;
            case AbilityCondition.HasWeapon:
                Debug.Log("이건 나중에 처리하자");
                break;
        }
    }

    private void ApplyTarget(CardAbilityData ability, IEntity self = null, IEntity target = null)
    {
        targets = new List<IEntity>();
        var battleManager = Locator<BattleManager>.Get();
        var battleField = battleManager.GetBattleField();
        var battleFieldComponent = battleField.GetComponent<BattleField>();

        // Ability Target 
        switch (ability.target)
        {
            case AbilityTarget.TargetFriendlyMinion:
            case AbilityTarget.TargetEnemyMinion:
            case AbilityTarget.AllTarget:
            case AbilityTarget.AllTargetMinion:
                targets.Add(target);
                break;
            case AbilityTarget.AllFriendlyMinions:
                targets = battleFieldComponent.PlayerField<IEntity>();
                targets.Add(self);
                break;
            case AbilityTarget.AllEnemyMinions:
                targets = battleFieldComponent.EnemyField<IEntity>();
                break;
            case AbilityTarget.AllMinions:
                targets = battleFieldComponent.AllField<IEntity>();
                break;
            case AbilityTarget.RandomFriendlyMinion:
                targets = battleFieldComponent.PlayerField<IEntity>();
                break;
            case AbilityTarget.RandomEnemyMinion:
                targets = battleFieldComponent.EnemyField<IEntity>();
                break;
            case AbilityTarget.AllRandomMinion:
                targets = battleFieldComponent.AllField<IEntity>();
                break;
            case AbilityTarget.FriendlyHero:
                targets.Add(battleFieldComponent.playerHero);
                break;
            case AbilityTarget.EnemyHero:
                targets.Add(battleFieldComponent.enemyHero);
                break;
            case AbilityTarget.AllHeroes:
                targets.Add(battleFieldComponent.playerHero);
                targets.Add(battleFieldComponent.enemyHero);
                break;
            case AbilityTarget.AllFriendlyMinionsExceptSelf:
                targets = battleFieldComponent.PlayerField<IEntity>(self);
                break;
            case AbilityTarget.AllMinionsExceptSelf:
                targets = battleFieldComponent.AllField<IEntity>(self);
                break;
            case AbilityTarget.AdjacentMinions:
                Debug.Log("근처 적을 공격할 수 있는 방법이 있을까?");
                break;
            case AbilityTarget.FriendlyHand:
                Debug.Log("내 핸드를 확인할 수 있는 방법이 있을까?");
                break;
            case AbilityTarget.EnemyHand:
                Debug.Log("적 핸드를 확인할 수 있는 방법이 있을까?");
                break;
            case AbilityTarget.FriendlyDeck:
                Debug.Log("내 덱을 확인할 수 있는 방법이 있을까?");
                break;
            case AbilityTarget.EnemyDeck:
                Debug.Log("적 덱을 확인할 수 있는 방법이 있을까?");
                break;
            case AbilityTarget.Self:
                targets.Add(self);
                break;
        }
    }

    private void Over(CardAbilityData ability)
    {
        var conditionList = new List<IEntity>();
        foreach (var entity in targets)
        {
            IStat stat = null;
            if (entity is IStat entityStat)
                stat = entityStat;
            else
                continue;

            switch (ability.conditionState)
            {
                case AbilityConditionStat.Attack:
                    int attack = stat.GetAttackPoint();
                    if (ability.conditionValue <= attack)
                        conditionList.Add(entity);
                    break;
                case AbilityConditionStat.Health:
                    int health = stat.GetHealthPoint();
                    if (ability.conditionValue <= health)
                        conditionList.Add(entity);
                    break;
                case AbilityConditionStat.Cost:
                    int cost = stat.GetCost();
                    if (ability.conditionValue <= cost)
                        conditionList.Add(entity);
                    break;
            }
        }

        targets = conditionList;
    }

    private void Under(CardAbilityData ability)
    {
        var conditionList = new List<IEntity>();
        foreach (var entity in targets)
        {
            IStat stat = null;
            if (entity is IStat entityStat)
                stat = entityStat;
            else
                continue;

            switch (ability.conditionState)
            {
                case AbilityConditionStat.Attack:
                    int attack = stat.GetAttackPoint();
                    if (ability.conditionValue >= attack)
                        conditionList.Add(entity);
                    break;
                case AbilityConditionStat.Health:
                    int health = stat.GetHealthPoint();
                    if (ability.conditionValue >= health)
                        conditionList.Add(entity);
                    break;
                case AbilityConditionStat.Cost:
                    int cost = stat.GetCost();
                    if (ability.conditionValue >= cost)
                        conditionList.Add(entity);
                    break;
            }
        }

        targets = conditionList;
    }

    private void MinionType(CardAbilityData ability)
    {
        var conditionList = new List<IEntity>();
        foreach (var entity in targets)
        {
            IStat stat = null;
            if (entity is IStat entityStat)
                stat = entityStat;
            else
                continue;

            var cardTypes = stat.GetCardTypes();

            if(cardTypes != null)
            {
                bool hasType = false;
                foreach(var type in cardTypes)
                {
                    if (ability.conditionType == type)
                        hasType = true;
                }

                if (!hasType)
                    continue;
            }    


            switch (ability.conditionState)
            {
                case AbilityConditionStat.Attack:
                    int attack = stat.GetAttackPoint();
                    if (ability.conditionValue >= attack)
                        conditionList.Add(entity);
                    break;
                case AbilityConditionStat.Health:
                    int health = stat.GetHealthPoint();
                    if (ability.conditionValue >= health)
                        conditionList.Add(entity);
                    break;
                case AbilityConditionStat.Cost:
                    int cost = stat.GetCost();
                    if (ability.conditionValue >= cost)
                        conditionList.Add(entity);
                    break;
                default:
                    conditionList.Add(entity);
                    break;
            }
        }

        targets = conditionList;
    }

    // SpellType은 잘 모르겠네
    private void SpellType(CardAbilityData ability)
    {
        var conditionList = new List<IEntity>();
        foreach (var entity in targets)
        {
            IStat stat = null;
            if (entity is IStat entityStat)
                stat = entityStat;
            else
                continue;

            var cardTypes = stat.GetCardTypes();

            if (cardTypes != null)
            {
                bool hasType = false;
                foreach (var type in cardTypes)
                {
                    if (ability.conditionType == type)
                        hasType = true;
                }

                if (!hasType)
                    continue;
            }

            switch (ability.conditionState)
            {
                case AbilityConditionStat.Attack:
                    int attack = stat.GetAttackPoint();
                    if (ability.conditionValue >= attack)
                        conditionList.Add(entity);
                    break;
                case AbilityConditionStat.Health:
                    int health = stat.GetHealthPoint();
                    if (ability.conditionValue >= health)
                        conditionList.Add(entity);
                    break;
                case AbilityConditionStat.Cost:
                    int cost = stat.GetCost();
                    if (ability.conditionValue >= cost)
                        conditionList.Add(entity);
                    break;
                default:
                    conditionList.Add(entity);
                    break;
            }
        }

        targets = conditionList;
    }

    private void Damage(int value)
    {
        // foreach를 돌면서 확인할건데 중간에 데미지입고 사라지는 경우도 있어서 
        // foreach의 안정성을 위해 복사본으로 돌린다.
        var cloneList = new List<IEntity>(targets);
        var clash = new Clash();
        foreach(var clone in cloneList) {
            if(clone is IDamageable damage) {
                damage.TakeDamage(value);
            }
        }
    }

    private void Heal(int value)
    {
        foreach(var entity in targets) {
            if(entity is IHealable heal) {
                heal.Heal(value);
            }
        }
    }

    private void Remove()
    {
        Debug.Log("Romove에 도착");
    }

    private void DrawCard()
    {
        Debug.Log("Draw Card에 도착");
    }

    private void ExecuteKeyword(KeywordType type)
    {
        foreach (var entity in targets)
        {
            if (entity is IKeyword keyword)
                keyword.AddKeyword(new KeywordEffect(type, isTempory));
        }
    }

    private void Token()
    {
        Debug.Log("Token에 도착");
    }

    private void Buffer(Vector2Int stat)
    {
        Debug.Log("Buffer 부여");
        foreach(var entity in targets) {
            if(entity is IBuffer buffer) {
                buffer.AddBuffer(new StatModifier(stat, isTempory));
            }
        }
    }
}
