using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Card/AbilityData")]
public class ScriptableAbilityData : ScriptableObject
{
    public List<AbilityData> abilityData = new List<AbilityData>();
}