public class Clash
{
    public void Execute(ICombatable attacker, ICombatable target)
    {
        // 대상에게 공격한다.
        target.TakeDamage(attacker.GetAttackPoint());

        // 공격력이 음수나 0이 아니라면 데미지를 입힌다.
        if (target.GetAttackPoint() > 0)
            attacker.TakeDamage(target.GetAttackPoint());
    }

    // IAttacker로 공격할 수도 있지만 battlecry 같은 경우에도 바로 공격할 수도 있다. 
    public void Execute(int attack, ICombatable target)
    {
        target.TakeDamage(attack);
    }
}
