using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ServantBattleState : EnemyState
{
    private Transform player;
    private Enemy_Servant enemy;
    private int moveDir;

    //가중치
    public float weightAtk1 = 1f;
    public float weightAtk2 = 1f;

    [SerializeField] private float stopThresholdX = 0.8f; // 수평 정지 임계값
    [SerializeField] private float stopThresholdY = 0.6f; // 수직 정지 임계값

    private bool flippedOnce;
    public ServantBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Servant _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;

        if (player.GetComponent<PlayerStats>().isDead)
            stateMachine.ChangeState(enemy.moveState);

        stateTimer = enemy.battleTime;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        // 위치 차이 계산
        float dx = player.position.x - enemy.transform.position.x;
        float dy = player.position.y - enemy.transform.position.y;
        float distX = Mathf.Abs(dx);

        // 라인 오브 사이트(시야) 1회 캐시
        var hit = enemy.IsPlayerDetected();              // ← 프레임당 1회
        bool los = hit.collider != null;
        float losDist = hit.distance;
        float effDist = los ? losDist : distX;           // 시야 있으면 레이 거리, 없으면 X거리

        // 공격 가능 범위(근접 위주) 판정
        bool inAttackRange = effDist <= enemy.attackDistance;

        // 공격 시도
        if (inAttackRange && CanAttack())
        {
            stateMachine.ChangeState(ChooseAttackState());
            return;
        }

        // XY가 모두 충분히 가까울 때 멈춤
        if (Mathf.Abs(dx) < stopThresholdX && Mathf.Abs(dy) < stopThresholdY)
        {
            enemy.SetVelocity(0f, rb.velocity.y);
            return;
        }

        // 이동 방향 결정 및 추격
        moveDir = dx >= 0 ? 1 : -1;

        enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y);
    }

    private bool CanAttack()
    {
        if (Time.time >= enemy.lastTimeAttacked + enemy.attackCooldown)
        {
            enemy.attackCooldown = Random.Range(enemy.minAttackCooldown, enemy.maxAttackCooldown);
            enemy.lastTimeAttacked = Time.time;
            return true;
        }

        return false;
    }

    private EnemyState ChooseAttackState()
    {
        // attackState2가 없을 수 있는 경우를 대비한 안전 처리
        if (enemy.attackState2 == null || weightAtk2 <= 0f)
            return enemy.attackState;

        if (enemy.attackState == null || weightAtk1 <= 0f)
            return enemy.attackState2;

        float total = weightAtk1 + weightAtk2;
        float r = Random.Range(0f, total);

        if (r < weightAtk1)
            return enemy.attackState;
        else
            return enemy.attackState2;
    }
}
