using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSummonState : EnemyState
{
    private readonly Enemy_Boss enemy;

    public BossSummonState(Enemy enemyBase, EnemyStateMachine sm, string animBool, Enemy_Boss _enemy) : base(enemyBase, sm, animBool)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled) // AnimationTrigger() 신호
        {
            enemy.nextSummonTime = Time.time + enemy.baseSummonCooldown; // 쿨타임 확정
            stateMachine.ChangeState(enemy.battleState);
        }

        if ((stateTimer -= Time.deltaTime) <= 0f)
            stateMachine.ChangeState(enemy.battleState);
    }

    public override void Exit()
    {
        base.Exit();
    }
}

