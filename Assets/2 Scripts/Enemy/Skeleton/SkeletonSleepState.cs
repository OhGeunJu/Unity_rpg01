using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonSleepState : SkeletonGroundedState
{
    public SkeletonSleepState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy)
        : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        enemy.anim.speed = 0f;

        enemy.healthBar.SetVisible(false);
    }

    public override void Update()
    {
        base.Update();

        enemy.SetZeroVelocity();

        // 여기서만 감지
        if (enemy.IsPlayerDetected() || Vector2.Distance(enemy.transform.position, player.position) < enemy.agroDistance)
        {
            enemy.isSleeping = false;
            stateMachine.ChangeState(enemy.digState);
        }
    }
}
