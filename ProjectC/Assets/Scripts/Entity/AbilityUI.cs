
public class AbilityUI
{
    public AbilityUI(UnityEngine.GameObject _ability, UnityEngine.Vector3 localPosition)
    {
        ability = _ability;
        rectTransform = _ability.GetComponent<UnityEngine.RectTransform>();
        abilityLocalPosition = localPosition;
    }

    public UnityEngine.GameObject ability;
    public UnityEngine.RectTransform rectTransform;
    public UnityEngine.Vector3 abilityLocalPosition;
}

public enum AbilityType
{
    Taunt,
    DivineShield,
    Stealth,
    Windfury,
    TurnEffect,
    Deathrattle,
    Freeze,
}
