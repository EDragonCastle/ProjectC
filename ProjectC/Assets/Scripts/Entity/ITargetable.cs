using UnityEngine.EventSystems;

public interface ITargetable
{
    public bool isTargetable { get; }
    public void OnTargeted();
    public void OnUnTargeted();
    public void OnPointerDown(); 
}

public interface IDamageable
{
    public void TakeDamage(int value);
}

// 서로 공격해야 해서 둘이 합쳐진 Interface가 필요하다.
public interface ICombatable : IDamageable, IStat
{

}

public interface IHealable
{
    public void Heal(int value);
}

public interface EntityController : IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    // Entity를 가져올 방법이 필요하다.
    public UnityEngine.MonoBehaviour GetEntity();
}

public interface IStat
{
    public int GetAttackPoint();
    public int GetHealthPoint();
    public int GetCost();
    public string[] GetCardTypes();
}

public interface IBuffer
{
    public void AddBuffer(StatModifier stat);
    public void RemoveTemporaryBuffer();
}

public interface IKeyword
{
    public void AddKeyword(KeywordEffect keyword);
    public void RemoveTemporaryKeyword();
    public bool HasKeyword(KeywordType keyword);
}

public interface ITrigger
{
    public void OnTurnStart();
    public void OnTurnEnd();
    public void OnDeath();
    public void OnAttacking();
    public void OnAttacked();
    public void OnMinionSummon();
    public void OnSpellCast();
}

public enum KeywordType
{
    Taunt,
    DivineShield,
    Charge,
    Stealth,
    Windfury,
    Deathrattle,
}

public struct StatModifier
{
    public bool isTempory;
    public UnityEngine.Vector2Int stat; 

    public StatModifier(UnityEngine.Vector2Int _stat, bool _isTempory)
    {
        stat = _stat;
        isTempory = _isTempory;
    }

    public StatModifier(int attack, int health, bool _isTempory)
    {
        stat = new UnityEngine.Vector2Int(attack, health);
        isTempory = _isTempory;
    }
}

public struct KeywordEffect
{
    public KeywordType type;
    public bool isTemporary;

    public KeywordEffect(KeywordType _type, bool _isTemporary)
    {
        type = _type;
        isTemporary = _isTemporary;
    }
}
