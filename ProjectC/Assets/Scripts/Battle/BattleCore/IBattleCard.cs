using UnityEngine;
using Cysharp.Threading.Tasks;

public interface IBattleCard
{
    public UniTask Execute(BattleFieldObjectInformation battleCard);
}

public interface IMinion : IBattleCard
{
    // 아직은 잘 모르겠다.
}

public interface ISpell : IBattleCard
{
    // 아직은 잘 모르겠다.
}

public interface IWeapon : IBattleCard
{
    // 아직은 잘 모르겠다.
}

