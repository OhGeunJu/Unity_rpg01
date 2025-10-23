using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBringerBattleState : EnemyState
{
    private Enemy_DeathBringer enemy;
    private Transform player;
    private int moveDir;

    public DeathBringerBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_DeathBringer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;

        //if (player.GetComponent<PlayerStats>().isDead)
            //stateMachine.ChangeState(enemy.moveState);


    }

    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerDetected())
        {
            stateTimer = enemy.battleTime;

            if (enemy.IsPlayerDetected().distance < enemy.attackDistance) // 공격 거리 이내
            {
                if (CanAttack()) // 공격 가능하면
                    stateMachine.ChangeState(enemy.attackState); // 공격 상태로 전환
                else
                    stateMachine.ChangeState(enemy.idleState); // 공격 쿨타임이면 대기 상태로 전환
            }
        }

        if (player.position.x > enemy.transform.position.x) // 방향 전환
            moveDir = 1;
        else if (player.position.x < enemy.transform.position.x) // 방향 전환
            moveDir = -1;

        if (enemy.IsPlayerDetected() && enemy.IsPlayerDetected().distance < enemy.attackDistance - .1f) 
            return; // 너무 가까우면 멈춤

        enemy.SetVelocity(enemy.moveSpeed * moveDir, rb.velocity.y); // 플레이어 쪽으로 이동
    }

    public override void Exit()
    {
        base.Exit();
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
}
