using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class BattleField : MonoBehaviour, IChannel
{
    // 이 곳이 하수인들이 싸울 전장이다.
    public GameObject enemyField;
    public GameObject playerField;

    public IEntity enemyHero;
    public IEntity playerHero;

    public GameObject enemyWeapon;
    public GameObject playerWeapon;

    private void Awake()
    {
        var battleManager = Locator<BattleManager>.Get();
        battleManager.SetBattleField(this.gameObject);
    }

    private void OnEnable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Subscription(ChannelInfo.UsingBattleCard, HandleEvent);
    }

    private void OnDisable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Unsubscription(ChannelInfo.UsingBattleCard, HandleEvent);
    }
 

    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch(channel)
        {
            case ChannelInfo.UsingBattleCard:
                if (information is BattleFieldObjectInformation battleInfo)
                {
                    GameObject card = battleInfo.card;
                    ExecuteCard(card, battleInfo);
                }
                break;
        }
    }

    private void ExecuteCard(GameObject card, BattleFieldObjectInformation battleInfo)
    {
        var battleComponent = card.GetComponent<BattleCard>();
        var ICardType = battleComponent.GetBattleCardType();
        ICardType?.Execute(battleInfo).Forget();
    }

    // 여기서 그러면 List<T>로 메서드 타입만 Generic Type으로 해도 되려나?
    // 근데 Generic Type으로 하는 것보다 메서드를 하나 만드는 것도 나쁘지 않아 보이기도 해.

    /// <summary>
    /// 적 필드에 있는 Entity를 가져온다.
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// IEntity를 구현한 Class의 interface만 유효하다.
    /// 0622 유효한 타입 : ITargetable, ICombatable, IEntity, IHealable, IBuffer, IKeyword, ITrigger
    /// </typeparam>
    /// 
    /// <returns>
    /// 적 필드에 있는 T 타입의 Entity List. 유효하지 않은 타입을 입력하면 빈 List를 반환한다.
    /// </returns>
    public List<T> EnemyField<T>() where T : class
    {
        List<T> children = new List<T>();

        foreach(Transform child in enemyField.transform) {
            var cardEntity = child.gameObject.GetComponent<T>();
            if (cardEntity != null)
                children.Add(cardEntity);
        }
        return children;
    }

    public List<T> EnemyField<T>(T self) where T : class
    {
        List<T> children = new List<T>();

        foreach (Transform child in enemyField.transform)
        {
            var cardEntity = child.gameObject.GetComponent<T>();
            if (cardEntity != null) {
                if(self != cardEntity)
                    children.Add(cardEntity);
            }
        }
        return children;
    }

    public List<ITargetable> EnemyFieldChildren()
    {
        List<ITargetable> children = new List<ITargetable>();
    
        foreach(Transform child in enemyField.transform) {
            var cardEntity = child.gameObject.GetComponent<ITargetable>();
            if(cardEntity != null)
                children.Add(cardEntity);
        }

        return children;
    }

    public List<ITargetable> EnemyFieldChildren(ITargetable self)
    {
        List<ITargetable> children = new List<ITargetable>();

        foreach (Transform child in enemyField.transform)
        {
            var cardEntity = child.gameObject.GetComponent<ITargetable>();
            if (cardEntity != null) {
                if(self != cardEntity)
                    children.Add(cardEntity);
            }
        }

        return children;
    }

    public List<T> PlayerField<T>() where T : class
    {
        List<T> children = new List<T>();

        foreach (Transform child in playerField.transform)
        {
            var cardEntity = child.gameObject.GetComponent<T>();
            if (cardEntity != null)
                children.Add(cardEntity);
        }
        return children;
    }

    public List<T> PlayerField<T>(T self) where T : class
    {
        List<T> children = new List<T>();

        foreach (Transform child in playerField.transform)
        {
            var cardEntity = child.gameObject.GetComponent<T>();
            if (cardEntity != null) {
                if (self != cardEntity)
                    children.Add(cardEntity);
            }
        }
        return children;
    }

    public List<T> AllField<T>() where T : class
    {
        List<T> children = new List<T>();

        foreach (Transform child in enemyField.transform)
        {
            var cardEntity = child.gameObject.GetComponent<T>();
            if (cardEntity != null)
                children.Add(cardEntity);
        }

        foreach (Transform child in playerField.transform)
        {
            var cardEntity = child.gameObject.GetComponent<T>();
            if (cardEntity != null)
                children.Add(cardEntity);
        }
        return children;
    }

    public List<T> AllField<T>(T self) where T : class
    {
        List<T> children = new List<T>();

        foreach (Transform child in playerField.transform)
        {
            var cardEntity = child.gameObject.GetComponent<T>();
            if (cardEntity != null) {
                if (self != cardEntity)
                    children.Add(cardEntity);
            }
        }

        foreach (Transform child in enemyField.transform)
        {
            var cardEntity = child.gameObject.GetComponent<T>();
            if (cardEntity != null) {
                if (self != cardEntity)
                    children.Add(cardEntity);
            }
        }

        return children;
    }


    public List<ITargetable> PlayerFieldChildren()
    {
        List<ITargetable> children = new List<ITargetable>();

        foreach (Transform child in playerField.transform) {
            var cardEntity = child.gameObject.GetComponent<ITargetable>();
            if (cardEntity != null)
                children.Add(cardEntity);
        }

        return children;
    }

    public List<ITargetable> PlayerFieldChildren(ITargetable self)
    {
        List<ITargetable> children = new List<ITargetable>();

        foreach (Transform child in playerField.transform)
        {
            var cardEntity = child.gameObject.GetComponent<ITargetable>();
            if (cardEntity != null) {
                if (self != cardEntity)
                    children.Add(cardEntity);
            }
        }

        return children;
    }


    public List<ITargetable> AllFieldChildren()
    {
        List<ITargetable> children = new List<ITargetable>();

        foreach (Transform child in playerField.transform) {
            var cardEntity = child.gameObject.GetComponent<ITargetable>();
            if(cardEntity != null)
                children.Add(cardEntity);
        }

        foreach (Transform child in enemyField.transform) {
            var cardEntity = child.gameObject.GetComponent<ITargetable>();
            if (cardEntity != null)
                children.Add(cardEntity);
        }

        return children;
    }

    public List<ITargetable> AllFieldChildren(ITargetable self)
    {
        List<ITargetable> children = new List<ITargetable>();

        foreach (Transform child in playerField.transform)
        {
            var cardEntity = child.gameObject.GetComponent<ITargetable>();
            if (cardEntity != null)
            {
                if (self != cardEntity)
                    children.Add(cardEntity);
            }
        }

        foreach (Transform child in enemyField.transform)
        {
            var cardEntity = child.gameObject.GetComponent<ITargetable>();
            if (cardEntity != null)
            {
                if (self != cardEntity)
                    children.Add(cardEntity);
            }
        }

        return children;
    }

    private GameObject GetWeaponSlot(bool isPlayer)
    {
        return isPlayer ? playerWeapon : enemyWeapon;
    }

    public GameObject PlaceWeapon(GameObject weaponPrefab, bool isPlayer)
    {
        var slot = GetWeaponSlot(isPlayer);

        if(slot.transform.childCount > 0)
        {
            var weapon = slot.transform.GetChild(0).GetComponent<WeaponEntity>();
            weapon?.DestroyWeapon();
        }

        return Instantiate(weaponPrefab, slot.transform);
        
    }
}

public struct BattleFieldObjectInformation
{
    public GameObject card;
    public IBattleCard cardType;
    public BattleCardAbilityData ability;
    public bool isPlayer;
    public int battleIndex;
    public int usingIndex;
}


// 하수인 실행 -> 결정