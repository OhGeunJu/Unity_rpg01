using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServantDeadState : EnemyState
{
    Enemy_Servant enemy;
    public ServantDeadState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Servant _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        var hpUI = enemy.GetComponentInChildren<UI_HealthBar>(true);
        if (hpUI) hpUI.gameObject.SetActive(false);
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
