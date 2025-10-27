using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boss : Enemy
{
    public bool bossFightBegun;

    [SerializeField] public GameObject bossUI;

    public BossIdleState idleState { get; private set; }
    public BossBattleState battleState { get; private set; }
    public BossAttackState attackState { get; private set; }
    public BossAttackState2 attackState2 { get; private set; }
    public BossDeadState deadState { get; private set; }

    [Header("Boss Attacks")]
    public float atk1Cooldown = 2f;
    public float atk2Cooldown = 5f;
    [HideInInspector] public float lastAtk1Time = -999f;
    [HideInInspector] public float lastAtk2Time = -999f;

    public float atk1MinRange = 0f, atk1MaxRange = 2.2f; // 근접
    public float atk2MinRange = 0f, atk2MaxRange = 2.2f;   // 중/원거리
    public bool atk1RequireLOS = false;
    public bool atk2RequireLOS = false;

    public int currentAttackId { get; set; } // 1 또는 2 (애니 이벤트 분기용)

    protected override void Awake()
    {
        base.Awake();

        idleState = new BossIdleState(this, stateMachine, "Idle", this);
        battleState = new BossBattleState(this, stateMachine, "Move", this);
        attackState = new BossAttackState(this, stateMachine, "Attack", this);
        attackState2 = new BossAttackState2(this, stateMachine, "Attack2", this);
        deadState = new BossDeadState(this, stateMachine, "Dead", this);
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
        base.Die();
        stateMachine.ChangeState(deadState);

    }

    public bool CanUseAttack1(float dist, bool los)
        => Time.time >= lastAtk1Time + atk1Cooldown
           && dist >= atk1MinRange && dist <= atk1MaxRange
           && (!atk1RequireLOS || los);

    public bool CanUseAttack2(float dist, bool los)
        => Time.time >= lastAtk2Time + atk2Cooldown
           && dist >= atk2MinRange && dist <= atk2MaxRange
           && (!atk2RequireLOS || los);

    public void StampAttack1Cooldown() => lastAtk1Time = Time.time;
    public void StampAttack2Cooldown() => lastAtk2Time = Time.time;
}
