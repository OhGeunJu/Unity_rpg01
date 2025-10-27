using UnityEngine;

public class BossBattleState : EnemyState
{
    private Enemy_Boss enemy;
    private Transform player;
    private int moveDir;

    [SerializeField] private float weightAtk1 = 1f;
    [SerializeField] private float weightAtk2 = 1f;

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
        // 1) 플레이어 방향/거리
        float dx = player.position.x - enemy.transform.position.x;
        moveDir = dx >= 0 ? 1 : -1;
        float distX = Mathf.Abs(dx);

        // 2) 라인오브사이트 1회 캐시
        var hit = enemy.IsPlayerDetected();
        bool los = hit.collider != null;
        float losDist = hit.distance;

        // 3) 공격 범위 판정
        bool inAttackRange =
            (los && losDist <= enemy.attackDistance) ||
            (!los && distX <= enemy.attackDistance);

        if (inAttackRange)
        {
            // 4) 공격 가능 후보 수집 (각 쿨다운/거리/LOS 조건 반영)
            bool can1 = enemy.CanUseAttack1(los ? losDist : distX, los);
            bool can2 = enemy.CanUseAttack2(los ? losDist : distX, los);

            if (can1 || can2)
            {
                enemy.SetZeroVelocity();

                // 가중치 랜덤 (가능한 것만 포함)
                float w1 = can1 ? weightAtk1 : 0f;
                float w2 = can2 ? weightAtk2 : 0f;
                float total = w1 + w2;

                // 안전 체크
                if (total > 0f)
                {
                    float r = Random.value * total;
                    if (r <= w1)
                        stateMachine.ChangeState(enemy.attackState);   // 공격1: Enter에서 쿨다운 스탬프
                    else
                        stateMachine.ChangeState(enemy.attackState2);  // 공격2: Enter에서 쿨다운 스탬프
                    return;
                }
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
    private bool IsOffCooldown()
    {
        return Time.time >= enemy.lastTimeAttacked + enemy.attackCooldown;
    }

    private void SetupNextCooldown()
    {
        enemy.attackCooldown = Random.Range(enemy.minAttackCooldown, enemy.maxAttackCooldown);
        enemy.lastTimeAttacked = Time.time;
    }

    private void EnterSelectedAttack()
    {
        // 우선순위 조건이 있으면 여기서 먼저 리턴
        // if (특수상황) { SetupNextCooldown(); stateMachine.ChangeState(enemy.attackState2); return; }

        // 가중치 랜덤(둘 다 가능할 때만 의미 있음)
        float total = weightAtk1 + weightAtk2;
        float r = Random.value * total;

        SetupNextCooldown();

        if (r <= weightAtk1)
            stateMachine.ChangeState(enemy.attackState);   // 기존 공격
        else
            stateMachine.ChangeState(enemy.attackState2);  // 새 공격
    }
}
