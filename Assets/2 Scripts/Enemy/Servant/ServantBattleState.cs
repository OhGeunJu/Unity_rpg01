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
        flippedOnce = false;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerDetected())
        {
            stateTimer = enemy.battleTime;

            if (enemy.IsPlayerDetected().distance < enemy.attackDistance)
            {
                if (CanAttack())
                    stateMachine.ChangeState(ChooseAttackState());
            }
        }
        else
        {
            if (flippedOnce == false)
            {
                flippedOnce = true;
                enemy.Flip();
            }

            if (stateTimer < 0 || Vector2.Distance(player.transform.position, enemy.transform.position) > 7)
                stateMachine.ChangeState(enemy.idleState);
        }

        float distanceToPlayerX = Mathf.Abs(player.position.x - enemy.transform.position.x);

        if (distanceToPlayerX < .8f)
            return;

        if (player.position.x > enemy.transform.position.x)
            moveDir = 1;
        else if (player.position.x < enemy.transform.position.x)
            moveDir = -1;


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
