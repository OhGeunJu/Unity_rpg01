using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Enemy_Servant : Enemy
{
    public bool servantFightBegun;

    public ServantIdleState idleState { get; private set; }
    public ServantMoveState moveState { get; private set; }
    public ServantBattleState battleState { get; private set; }
    public ServantAttackState attackState { get; private set; }
    public ServantAttackState2 attackState2 { get; private set; }
    public ServantDeadState deadState { get; private set; }

    private Enemy_Boss _owner;
    private BossSummonController _controller;

    public void Init(Enemy_Boss boss, BossSummonController controller)
    {
        _owner = boss;
        _controller = controller;

        // 필요한 초기화(체력/AI/타깃 등) 여기에서 재설정
        // 예) hp = maxHp; agent.Warp(transform.position); animator.Rebind(); 등
    }

    protected override void Awake()
    {
        base.Awake();

        idleState = new ServantIdleState(this, stateMachine, "Idle", this);
        moveState = new ServantMoveState(this, stateMachine, "Move", this);
        battleState = new ServantBattleState(this, stateMachine, "Move", this);
        attackState = new ServantAttackState(this, stateMachine, "Attack", this);
        attackState2 = new ServantAttackState2(this, stateMachine, "Attack2", this);
        deadState = new ServantDeadState(this, stateMachine, "Dead", this);
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Die()
    {
        _owner?.OnMinionDead();

        stateMachine.ChangeState(deadState);

    }
}
