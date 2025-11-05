using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServantIdleState : ServantGroundedState
{
    public ServantIdleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Servant _enemy) : base(_enemyBase, _stateMachine, _animBoolName, _enemy)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = enemy.idleTime;
        player = PlayerManager.instance.player.transform;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (Vector2.Distance(player.transform.position, enemy.transform.position) < 7)
            enemy.servantFightBegun = true;

        if (stateTimer < 0 && enemy.servantFightBegun)
            stateMachine.ChangeState(enemy.battleState);
    }
}
