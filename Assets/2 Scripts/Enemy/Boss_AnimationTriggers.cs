using UnityEngine;

public class Boss_AnimationTriggers : MonoBehaviour
{
    private Enemy_Boss enemy;
    private BossStats bossStats;

    void Awake()
    {
        enemy = GetComponentInParent<Enemy_Boss>();
        bossStats = GetComponentInParent<BossStats>();
    }

    private void AnimationTrigger()
    {
        enemy.AnimationFinishTrigger();
    }

    // 공격 1 (기본 근접 공격)
    private void AttackTrigger1()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.attackCheck.position, enemy.attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>() != null) // 플레이어와 충돌했을 때
            {
                PlayerStats target = hit.GetComponent<PlayerStats>();

                bossStats.DoDamage(target); // 일반 공격
            }
        }
    }

    // 공격 2 (강화형 / 특수 공격)
    private void AttackTrigger2()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.attackCheck.position, enemy.attackCheckRadius);
        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>() != null) // 플레이어와 충돌했을 때
            {
                PlayerStats target = hit.GetComponent<PlayerStats>();


                bossStats.DoBossDamage(target, 0.3f, 10); // 배율 0.3, 추가 데미지 10
            }
        }
    }

    private void SpeicalAttackTrigger() // 특수 공격 트리거
    {
        enemy.AnimationSpecialAttackTrigger();
    }

    // 선택적으로 카운터 윈도우가 필요하면 유지
    private void OpenCounterWindow() => enemy.OpenCounterAttackWindow();
    private void CloseCounterWindow() => enemy.CloseCounterAttackWindow();
}
