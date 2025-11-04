using UnityEngine;

public class BossBattleState : EnemyState
{
    private Enemy_Boss enemy;
    private Transform player;
    private int moveDir;

    [SerializeField] private float weightAtk1 = 1f; // 우선순위
    [SerializeField] private float weightAtk2 = 1f;
    [SerializeField] private float weightRanged = 2f; // 원거리(기본은 조금 더 우선)
    [SerializeField] private float weightSummon = 2f;

    public BossBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Boss _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        player = PlayerManager.instance.player.transform;

        if (enemy.bossUI != null)
            enemy.bossUI.gameObject.SetActive(true);
    }

    public override void Update()
    {
        base.Update();

        // 1) 플레이어 방향/거리
        float dx = player.position.x - enemy.transform.position.x;
        moveDir = dx >= 0 ? 1 : -1;
        float distX = Mathf.Abs(dx);

        // 2) 라인오브사이트 1회 캐시
        var hit = enemy.IsPlayerDetected();
        bool los = hit.collider != null;
        float losDist = hit.distance;
        float effDist = los ? losDist : distX; // 시야 있으면 Raycast 거리로, 없으면 단순 X거리로

        // 3) 근접 사거리 판정 (근접 공격에만 사용)
        bool inAttackRange =
            (los && losDist <= enemy.attackDistance) ||
            (!los && distX <= enemy.attackDistance);

        bool can1 = inAttackRange && enemy.CanUseAttack1(effDist, los);
        bool can2 = inAttackRange && enemy.CanUseAttack2(effDist, los);
        bool canSpell = enemy.CanDoSpellCast();
        bool canSummon = enemy.CanSummon();

        if (can1 || can2 || canSpell || canSummon)
        {
            enemy.SetZeroVelocity();

            // 가중치 랜덤 (가능한 것만 포함)
            float w1 = can1 ? weightAtk1 : 0f;
            float w2 = can2 ? weightAtk2 : 0f;
            float wr = canSpell ? weightRanged : 0f;
            float ws = canSummon ? weightSummon : 0f;
            float total = w1 + w2 + wr + ws;

            // 안전 체크
            if (total > 0f)
            {
                float r = Random.value * total;
                if (r <= w1)
                {
                    stateMachine.ChangeState(enemy.attackState);   // 공격1: Enter에서 쿨다운 스탬프
                }
                else if (r <= w1 + w2)
                {
                    stateMachine.ChangeState(enemy.attackState2);  // 공격2: Enter에서 쿨다운 스탬프
                }
                else if (r <= w1 + w2 + wr)
                {
                    stateMachine.ChangeState(enemy.spellCastState); // 원거리(새 상태)
                }
                else
                {
                    stateMachine.ChangeState(enemy.summonState);    // 소환
                }

                return;
            }


            // 공격 범위지만 둘 다 쿨이 안 돌았으면 제자리 유지
            enemy.SetZeroVelocity();
            return;
        }

        // 5) 추격 (너무 가까우면 정지)
        if (distX < 0.8f)
        {
            enemy.SetZeroVelocity();
            return;
        }

        // 추격(Flip은 SetVelocity -> FlipController에 의해 자동)
        enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);
    }

    public override void Exit()
    {
        base.Exit();
        // 필요 시 이동/버프 해제 등 정리
    }
}
