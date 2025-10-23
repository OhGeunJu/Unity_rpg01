using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AnimationTriggers : MonoBehaviour
{
    private Enemy enemy => GetComponentInParent<Enemy>();

    private void AnimationTrigger()
    {
        enemy.AnimationFinishTrigger();
    }

    private void AttackTrigger() // 공격 트리거
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemy.attackCheck.position, enemy.attackCheckRadius); // 공격 범위 내의 모든 콜라이더를 감지

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Player>() != null) // 플레이어와 충돌했을 때
            {
                PlayerStats target = hit.GetComponent<PlayerStats>();
                enemy.stats.DoDamage(target);
            }
        }
    }
    private void SpeicalAttackTrigger() // 특수 공격 트리거
    {
        enemy.AnimationSpecialAttackTrigger();
    }

    private void OpenCounterWindow() => enemy.OpenCounterAttackWindow(); // 카운터 어택 윈도우 열기
    private void CloseCounterWindow() => enemy.CloseCounterAttackWindow(); // 카운터 어택 윈도우 닫기
}
