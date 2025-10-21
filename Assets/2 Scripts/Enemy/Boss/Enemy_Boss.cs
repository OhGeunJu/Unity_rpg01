using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Enemy_Boss : Enemy
{
    public BossIdleState idleState { get; private set; }
    public BossMoveState moveState { get; private set; }
    public BossBattleState battleState { get; private set; }
    public BossAttackState attackState { get; private set; }
    public BossDeadState deadState { get; private set; }


    protected override void Awake()
    {
        deadState = new BossDeadState(this, stateMachine, "Idle", this);
    }

    protected override void Start()
    {
        base.Start();

    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Die()
    {
        base.Die();
        stateMachine.ChangeState(deadState);

    }
}
