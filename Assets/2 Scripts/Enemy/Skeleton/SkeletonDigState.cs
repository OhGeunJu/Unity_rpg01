using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemy_Skeleton;

public class SkeletonDigState : EnemyState
{
    private Enemy_Skeleton enemy;
    private Transform player;

    public SkeletonDigState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy)
        : base(_enemyBase, _stateMachine, _animBoolName)
    {
        enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;

        enemy.anim.speed = 1f;

        switch (enemy.enemyType)
        {
            case EnemyType.Skeleton:
                stateTimer = 1.6f; // 일반 스켈레톤
                break;

            case EnemyType.EliteSkeleton:
                stateTimer = 2.3f; // 엘리트 스켈레톤
                break;
        }
    }

    public override void Update()
    {
        base.Update();

        enemy.SetZeroVelocity();

        if (stateTimer < 0)
        {
            enemy.healthBar.SetVisible(true);

            // 플레이어와의 거리/각도에 따라 Idle 또는 Battle 선택 가능
            if (enemy.IsPlayerDetected() || Vector2.Distance(enemy.transform.position, player.transform.position) < enemy.agroDistance)
                stateMachine.ChangeState(enemy.battleState);
            else
                stateMachine.ChangeState(enemy.idleState);
        }
    }
}
