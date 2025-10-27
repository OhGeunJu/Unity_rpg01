using UnityEngine;

public class BossStats : EnemyStats
{
    public void DoBossDamage(CharacterStats target, float multiplier = 1f, int flatBonus = 0) // 보스 전용 데미지 함수
    {
        if (target == null || target.isInvincible) return;
        if (TargetCanAvoidAttack(target)) return;

        target.GetComponent<Entity>().SetupKnockbackDir(transform);

        int basePhys = damage.GetValue() + strength.GetValue();
        int total = Mathf.RoundToInt((basePhys + flatBonus) * multiplier);

        bool critical = false;
        if (CanCrit())
        {
            total = CalculateCriticalDamage(total);
            critical = true;
        }

        fx.CreateHitFx(target.transform, critical);

        total = CheckTargetArmor(target, total);
        target.TakeDamage(total);

        DoMagicalDamage(target);
    }
}
