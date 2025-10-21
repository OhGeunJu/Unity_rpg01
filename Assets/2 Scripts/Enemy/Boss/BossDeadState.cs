using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDeadState : EnemyState
{
    private Enemy_Boss enemy;

    public BossDeadState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Boss _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        //enemy.anim.SetBool(enemy.lastAnimBoolName, true);
        //enemy.anim.speed = 0;
        //enemy.cd.enabled = false;

        //stateTimer = .15f;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }
}
