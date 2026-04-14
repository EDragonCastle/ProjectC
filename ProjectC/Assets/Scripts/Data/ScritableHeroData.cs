using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewScritableHeroData", menuName = "Data/ScritableHeroData")]
public class ScritableHeroData : ScriptableObject
{
    public List<HeroData> heroData = new List<HeroData>();
}
