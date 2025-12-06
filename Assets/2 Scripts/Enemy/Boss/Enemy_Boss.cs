using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boss : Enemy
{
    public bool bossFightBegun;


    [SerializeField] public GameObject bossUI;

    public BossStats bossStats { get; private set; }
    public BossIdleState idleState { get; private set; }
    public BossBattleState battleState { get; private set; }
    public BossAttackState attackState { get; private set; }
    public BossAttackState2 attackState2 { get; private set; }
    public BossSpellCastState spellCastState { get; private set; }
    public BossSummonState summonState { get; private set; }
    public BossDeadState deadState { get; private set; }

    [Header("Boss Attacks")]
    public float atk1Cooldown = 2f;
    public float atk2Cooldown = 5f;
    [HideInInspector] public float lastAtk1Time = -999f;
    [HideInInspector] public float lastAtk2Time = -999f;

    public float atk1MinRange = 0f, atk1MaxRange = 2.2f; // 근접
    public float atk2MinRange = 0f, atk2MaxRange = 2.2f;

    [Header("Spell cast details")]
    [SerializeField] private GameObject spellPrefab;
    public int amountOfSpells;
    public float spellCooldown;
    public float lastTimeCast;
    [SerializeField] private float spellStateCooldown;
    [SerializeField] private Vector2 spellOffset;

    [Header("Summon Components")]
    [SerializeField] private BossSummonController summonController;

    [Header("Summon Rules")]
    [SerializeField] public int maxMinionsOnField = 6; // 동시 유지 최대
    [SerializeField] public int totalSummonBudget = 30;// 전투 중 총 소환 한도
    [SerializeField] public float baseSummonCooldown = 30f;

    [HideInInspector] public float nextSummonTime;
    [HideInInspector] public int totalSummoned;
    public int CurrentMinionCount { get; private set; }


    public int currentAttackId { get; set; } // 1 또는 2 (애니 이벤트 분기용)

    protected override void Awake()
    {
        base.Awake();

        idleState = new BossIdleState(this, stateMachine, "Idle", this);
        battleState = new BossBattleState(this, stateMachine, "Move", this);
        attackState = new BossAttackState(this, stateMachine, "Attack", this);
        attackState2 = new BossAttackState2(this, stateMachine, "Attack2", this);
        spellCastState = new BossSpellCastState(this, stateMachine, "SpellCast", this);
        summonState = new BossSummonState(this, stateMachine, "Summon", this);
        deadState = new BossDeadState(this, stateMachine, "Dead", this);
    }

    protected override void Start()
    {
        base.Start();
        bossStats = GetComponent<BossStats>();

        // 컨트롤러 초기화
        if (summonController != null)
            summonController.Init(this);

        // 첫 소환까지 랜덤 지연(선택)
        nextSummonTime = Time.time + Random.Range(5f, 8f);

        stateMachine.Initialize(idleState);

    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Die()
    {
        base.Die();

        UI ui = FindObjectOfType<UI>();
        if (ui != null)
            ui.SwitchOnWinScreen();

        stateMachine.ChangeState(deadState);

    }

    public bool CanUseAttack1(float dist, bool los)
        => Time.time >= lastAtk1Time + atk1Cooldown
           && dist >= atk1MinRange && dist <= atk1MaxRange;

    public bool CanUseAttack2(float dist, bool los)
        => Time.time >= lastAtk2Time + atk2Cooldown
           && dist >= atk2MinRange && dist <= atk2MaxRange;

    public void StampAttack1Cooldown() => lastAtk1Time = Time.time;
    public void StampAttack2Cooldown() => lastAtk2Time = Time.time;

    public void CastSpell()
    {
        Player player = PlayerManager.instance.player;


        float xOffset = 0;

        if (player.rb.velocity.x != 0)
            xOffset = player.facingDir * spellOffset.x;

        Vector3 spellPosition = new Vector3(player.transform.position.x + xOffset, player.transform.position.y + spellOffset.y);

        GameObject newSpell = Instantiate(spellPrefab, spellPosition, Quaternion.identity);
        newSpell.GetComponent<BossSpell_Controller>().SetupSpell(bossStats);
    }

    public bool CanDoSpellCast()
    {
        if (Time.time >= lastTimeCast + spellStateCooldown)
        {
            return true;
        }

        return false;
    }

    public bool CanSummon()
    {
        if (Time.time < nextSummonTime) return false;
        if (CurrentMinionCount >= maxMinionsOnField - 1) return false;
        if (totalSummonBudget > 0 && totalSummoned >= totalSummonBudget) return false;
        return true;
    }

    public void OnMinionSpawned(int n)
    {
        CurrentMinionCount += n;
        totalSummoned += n;
    }

    /// <summary>서번트 사망/소멸 시 반드시 호출</summary>
    public void OnMinionDead()
    {
        CurrentMinionCount = Mathf.Max(0, CurrentMinionCount - 1);
    }

}
