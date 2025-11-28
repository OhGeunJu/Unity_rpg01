using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Skeleton,
    EliteSkeleton
}

public class Enemy_Skeleton : Enemy
{

    #region States

    public SkeletonSleepState sleepState { get; private set; }
    public SkeletonDigState digState { get; private set; }
    public SkeletonIdleState idleState { get; private set; }
    public SkeletonMoveState moveState { get; private set; }
    public SkeletonBattleState battleState { get; private set; }
    public SkeletonAttackState attackState { get; private set; }

    public SkeletonStunnedState stunnedState { get; private set; }
    public SkeletonDeadState deadState { get; private set; }
    #endregion

    public EnemyType enemyType;

    public UI_HealthBar healthBar;

    public bool isSleeping = true;

    protected override void Awake()
    {
        base.Awake();
        
        sleepState = new SkeletonSleepState(this, stateMachine, "Dig", this);
        digState = new SkeletonDigState(this, stateMachine, "Dig", this);
        idleState = new SkeletonIdleState(this, stateMachine, "Idle", this);
        moveState = new SkeletonMoveState(this, stateMachine, "Move", this);
        battleState = new SkeletonBattleState(this, stateMachine, "Battle", this);
        attackState = new SkeletonAttackState(this, stateMachine, "Attack", this);
        stunnedState = new SkeletonStunnedState(this, stateMachine, "Stunned", this);
        deadState = new SkeletonDeadState(this, stateMachine, "Dead", this);
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(sleepState);
    }


    public override bool CanBeStunned()
    {
        if (base.CanBeStunned())
        {
            stateMachine.ChangeState(stunnedState);
            return true;
        }

        return false;
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
