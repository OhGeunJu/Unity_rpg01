using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        AudioManager.instance.PlaySFX(8,null);
    }

    public override void Exit()
    {
        base.Exit();

        AudioManager.instance.StopSFX(8);
    }

    public override void Update()
    {
        base.Update();

        if (player.IsKnocked)
        {
            player.SetZeroVelocity();
            return;
        }

        player.SetVelocity(xInput * player.moveSpeed, rb.velocity.y);
        
        if (xInput == 0 || player.IsWallDetected())
            stateMachine.ChangeState(player.idleState);

        //지상에서만, 이동 입력 있을 때만 턱 넘기
        if (player.IsGroundDetected() && Mathf.Abs(xInput) > 0.01f)
            player.StepClimb(Mathf.Sign(xInput));

        if (player.isStepping)
        {
            player.stepStateTimer -= Time.deltaTime;
            if (player.stepStateTimer <= 0)
                player.isStepping = false;
        }
    }
}
