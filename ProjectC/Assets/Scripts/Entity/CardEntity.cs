using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class CardEntity : MonoBehaviour, ITargetable, ICombatable, IEntity, IHealable, IBuffer, IKeyword, ITrigger, IChannel
{
    [Header("Card Ability")]
    public GameObject divineShield;
    public GameObject taunt;
    public GameObject stealth;
    public GameObject windfury;
    public GameObject turnEffect;
    public GameObject deathFair;

    private Dictionary<AbilityType, AbilityUI> abilityUI;

    [Header("Card Data")]
    public Image cardMask;
    public Image cardImage;
    public Image cardBackGround;

    public GameObject legandPortrait;

    public Image attackImage;
    public TextMeshProUGUI attackName;
    public Image healthImage;
    public TextMeshProUGUI healthName;

    public Image highlight;

    public BattleCard card;
    private string[] types;

    private int attack;
    private int health;
    private int maxHealth;
    private int cost;

    private bool hasAttackedThisTurn = false;
    private bool isPlayer = false;

    private List<StatModifier> bufferList = new List<StatModifier>();
    private List<KeywordEffect> keywordList = new List<KeywordEffect>();
    private List<CardAbilityData> originAbilityList;

    private Canvas parentCanvas;

    private Dictionary<AbilityTrigger, List<CardAbilityData>> sortedTriggerAbilitys = new Dictionary<AbilityTrigger, List<CardAbilityData>>();

    #region Interface
    #region ITargetable Interface 
    public bool isTargetable { get; private set; }

    public void OnTargeted()
    {
        Vector4 color = new Vector4(1, 1, 1, 1);
        highlight.color = color;
        isTargetable = true;
    }

    public void OnUnTargeted()
    {
        Vector4 color = new Vector4(1, 1, 1, 0.1f);
        highlight.color = color;
        isTargetable = false;
    }

    public void OnPointerDown()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.TargetSelected, this);
    }
    #endregion

    #region IDamageable Interface
    public void TakeDamage(int value)
    {
        health = health - value;
        healthName.text = health.ToString();
        OnAttacked();
        Damage();

        if (health <= 0)
            Die();
    }
    #endregion

    #region IHealable Interface
    public void Heal(int value)
    {
        health += value;

        if (maxHealth <= health) {
            health = maxHealth;
            Recovery();
        }

        healthName.text = health.ToString();
    }

    #endregion

    #region IStat Interface
    public int GetAttackPoint()
    {
        int total = attack;
        
        foreach(var buffer in bufferList) {
            total += buffer.stat.x;
        }

        return total;
    }
    public int GetHealthPoint()
    {
        int total = health;

        foreach (var buffer in bufferList) {
            total += buffer.stat.y;
        }

        return health;
    }

    public int GetCost()
    {
        return cost;
    }

    public string[] GetCardTypes()
    {
        return types;
    }
    #endregion

    #region IBuffer Interface
    public void AddBuffer(StatModifier stat)
    {
        bufferList.Add(stat);

        if (stat.stat.x != 0)
        {
            attackName.text = GetAttackPoint().ToString();
            attackImage.color = Color.green;
        }

        if (stat.stat.y != 0)
        {
            healthName.text = GetHealthPoint().ToString();
            healthName.color = Color.green;
        }
    }

    public void RemoveTemporaryBuffer()
    {
        bufferList.RemoveAll(m => m.isTempory);

        attackName.text = GetAttackPoint().ToString();
        healthName.text = GetHealthPoint().ToString();
    }
    #endregion

    #region IKeyword Interface
    public void AddKeyword(KeywordEffect keyword)
    {
        keywordList.Add(keyword);
        KeywordSetting(keyword.type);
    }

    public void RemoveTemporaryKeyword()
    {
        // RemoveAll 방식보다 다른 방식을 해야 할지도?
        keywordList.RemoveAll(m => m.isTemporary);
    }

    public bool HasKeyword(KeywordType keyword)
    {
        foreach(var innerkeyword in keywordList) {
            if (innerkeyword.type == keyword)
                return true;
        }
        return false;
    }
    #endregion

    #region ITrigger Interface
    public void OnTurnStart() => TriggerAbility(AbilityTrigger.OnTurnStart);
    public void OnTurnEnd() => TriggerAbility(AbilityTrigger.OnTurnEnd);
    public void OnDeath() => TriggerAbility(AbilityTrigger.Deathrattle);
    public void OnAttacking() => TriggerAbility(AbilityTrigger.OnAttacking);
    public void OnAttacked() => TriggerAbility(AbilityTrigger.OnAttacked);
    public void OnMinionSummon() => TriggerAbility(AbilityTrigger.OnMinionSummon);
    public void OnSpellCast() => TriggerAbility(AbilityTrigger.OnSpellCast);
    #endregion

    #region IChannel Interface
    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch(channel)
        {
            case ChannelInfo.OnTurnStart:
                // 내턴인지 적턴인지 알아야 한다.
                OnTurnStart();

                break;
            case ChannelInfo.OnTurnEnd:
                OnTurnEnd();
                RemoveTemporaryKeyword();
                RemoveTemporaryBuffer();
                break;
        }
    }

    #endregion

    #endregion

    private void LateUpdate()
    {
        if (abilityUI == null) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(this.transform.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            this.GetComponent<RectTransform>(),
            screenPos,
            parentCanvas.worldCamera,
            out Vector2 localPoint
        );


        foreach (var ability in abilityUI)
        {
            if (!ability.Value.ability.activeSelf) continue;

            ability.Value.rectTransform.localPosition = new Vector3(
                localPoint.x + ability.Value.abilityLocalPosition.x,
                localPoint.y + ability.Value.abilityLocalPosition.y,
                0
            );
        }
    }

    public void CardResourceSetting(MinionResourceData resourceData)
    {
        cardImage.sprite = resourceData.cardImage;
        legandPortrait.SetActive(resourceData.legand);
        attackName.text = resourceData.attack;
        healthName.text = resourceData.health;
    }

    // Player가 선택할 수 있는지 알 수 있는 방법
    public void EntitySetting(bool _isPlayer, List<CardAbilityData> abilitys)
    {
        isPlayer = _isPlayer;
        attack = int.Parse(attackName.text);
        maxHealth = int.Parse(healthName.text);
        health = maxHealth;
        types = card.GetAbilityData().GetCardTypes();
        parentCanvas = this.GetComponentInParent<Canvas>();

        originAbilityList = abilitys;
        SetAttacked();

        InitalizeAbilityUISetting();

        foreach (var ability in originAbilityList) {
            if (!sortedTriggerAbilitys.ContainsKey(ability.trigger))
                sortedTriggerAbilitys[ability.trigger] = new List<CardAbilityData>();

            sortedTriggerAbilitys[ability.trigger].Add(ability);  
        }

        SubScriptionEvent();
        TurnEffectDeathFairSetting();
    }
  
    public bool CanAttack()
    {
        return !hasAttackedThisTurn && attack > 0 && isPlayer;
    }

    public void SetAttacked()
    {
        hasAttackedThisTurn = true;
    }

    public void ResetAttack()
    {
        hasAttackedThisTurn = false;
    }

    private void InitalizeAbilityUISetting()
    {
        parentCanvas = this.GetComponentInParent<Canvas>();

        // Taunt localPosition 저장
        abilityUI = new Dictionary<AbilityType, AbilityUI>();
        abilityUI.Add(AbilityType.Taunt, new AbilityUI(taunt, taunt.GetComponent<RectTransform>().localPosition));
        abilityUI.Add(AbilityType.DivineShield, new AbilityUI(divineShield, divineShield.GetComponent<RectTransform>().localPosition));
        abilityUI.Add(AbilityType.Stealth, new AbilityUI(stealth, stealth.GetComponent<RectTransform>().localPosition));
        abilityUI.Add(AbilityType.Windfury, new AbilityUI(windfury, windfury.GetComponent<RectTransform>().localPosition));
        abilityUI.Add(AbilityType.TurnEffect, new AbilityUI(turnEffect, turnEffect.GetComponent<RectTransform>().localPosition));
        abilityUI.Add(AbilityType.Deathrattle, new AbilityUI(deathFair, deathFair.GetComponent<RectTransform>().localPosition));
    }

    private void SubScriptionEvent()
    {
        var eventManaer = Locator<EventManager>.Get();

        if(sortedTriggerAbilitys.ContainsKey(AbilityTrigger.OnTurnStart))
            eventManaer.Subscription(ChannelInfo.OnTurnStart, HandleEvent);

        if(sortedTriggerAbilitys.ContainsKey(AbilityTrigger.OnTurnEnd))
            eventManaer.Subscription(ChannelInfo.OnTurnEnd, HandleEvent);
    }

    private void TurnEffectDeathFairSetting()
    {
        if (sortedTriggerAbilitys.ContainsKey(AbilityTrigger.Deathrattle))
            deathFair.SetActive(true);

        if (sortedTriggerAbilitys.ContainsKey(AbilityTrigger.OnTurnEnd) || sortedTriggerAbilitys.ContainsKey(AbilityTrigger.OnTurnStart) || sortedTriggerAbilitys.ContainsKey(AbilityTrigger.OnAttacking) || sortedTriggerAbilitys.ContainsKey(AbilityTrigger.OnAttacked))
            turnEffect.SetActive(true);

        // 조건을 확인해야 한다.
        // 죽메랑 공격시랑 겹쳐 있으면 죽메가 숨겨지는 것을 볼 수 있다.
    }

    private void Die()
    {
        Debug.Log("이 하수인은 죽었다.");
        Destroy(this.gameObject);

        OnDeath();
    }

    private void Damage()
    {
        healthName.color = Color.red;
    }

    private void Recovery()
    {
        healthName.color = Color.white;
    }

    private void KeywordSetting(KeywordType type)
    {
        switch (type)
        {
            case KeywordType.Taunt:
                Debug.Log("도발 생성 로직");
                Taunt();
                break;
            case KeywordType.DivineShield:
                Debug.Log("천상의 보호막 생성 로직");
                DivineShield();
                break;
            case KeywordType.Charge:
                Debug.Log("돌진 생성 로직");
                ResetAttack();
                break;
            case KeywordType.Stealth:
                Debug.Log("은신 생성 로직");
                Stealth();
                break;
            case KeywordType.Windfury:
                Debug.Log("질풍 생성 로직");
                Windfury();
                break;
        }
    }

    // Trigger를 설정할 필요는 없다. 이미 TryGetValue에서 가져오기 때문이다.
    private void TriggerAbility(AbilityTrigger trigger)
    {
        var executor = new AbilityExecutor();
        var list = new List<CardAbilityData>();
        switch (trigger)
        {
            case AbilityTrigger.Deathrattle:
                if (sortedTriggerAbilitys.TryGetValue(AbilityTrigger.Deathrattle, out list))
                    executor.Execute(list, self: this);
                break;
            case AbilityTrigger.OnTurnEnd:
                if (sortedTriggerAbilitys.TryGetValue(AbilityTrigger.OnTurnEnd, out list))
                    executor.Execute(list, self: this);
                break;
            case AbilityTrigger.OnTurnStart:
                if (sortedTriggerAbilitys.TryGetValue(AbilityTrigger.OnTurnStart, out list))
                    executor.Execute(list, self: this);
                break;
            case AbilityTrigger.OnMinionSummon:
                if (sortedTriggerAbilitys.TryGetValue(AbilityTrigger.OnMinionSummon, out list))
                    executor.Execute(list, self: this);
                break;
            case AbilityTrigger.OnSpellCast:
                if (sortedTriggerAbilitys.TryGetValue(AbilityTrigger.OnSpellCast, out list))
                    executor.Execute(list, self: this);
                break;
            case AbilityTrigger.OnAttacking:
                if (sortedTriggerAbilitys.TryGetValue(AbilityTrigger.OnAttacking, out list))
                    executor.Execute(list, self: this);
                break;
            case AbilityTrigger.OnAttacked:
                if (sortedTriggerAbilitys.TryGetValue(AbilityTrigger.OnAttacked, out list))
                    executor.Execute(list, self: this);
                break;
            default:
                break;
        }
    }

    private void Taunt()
    {
        taunt.SetActive(true);
    }

    private void Stealth()
    {
        stealth.SetActive(true);
    }

    private void DivineShield()
    {
        divineShield.SetActive(true);
    }

    private void Windfury()
    {
        windfury.SetActive(true);
    }
}
