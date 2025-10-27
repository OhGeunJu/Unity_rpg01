using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackState2 : EnemyState
{
    private Enemy_Boss enemy;

    public BossAttackState2(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Boss _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }
    public override void Enter()
    {
        base.Enter();

        enemy.currentAttackId = 2;

        enemy.StampAttack2Cooldown();
    }

    public override void Exit()
    {
        base.Exit();

        enemy.lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();

        enemy.SetZeroVelocity();



        if (triggerCalled)
            stateMachine.ChangeState(enemy.battleState);
    }
}
