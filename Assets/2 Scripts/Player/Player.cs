using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : Entity
{
    [Header("Attack details")]
    public Vector2[] attackMovement;
    public float counterAttackDuration = .2f;

    public bool isBusy { get; private set; }
    [Header("Move info")]
    public float moveSpeed = 12f;
    public float jumpForce;
    public float swordReturnImpact;
    private float defaultMoveSpeed;
    private float defaultJumpForce;

    [Header("Dash info")]   
    public float dashSpeed;
    public float dashDuration;
    private float defaultDashSpeed;
    public float dashDir { get; private set; }

    [Header("Step Offset")]
    [SerializeField] private float stepHeight = 0.6f;        // 올라갈 수 있는 최대 턱 높이
    [SerializeField] private float stepCheckDistance = 0.05f; // 앞쪽 검사 거리
    [SerializeField] private float stepSmooth = 12f;          // 부드럽게 올라가는 속도
    public Transform stepLowerCheck;
    public Transform stepUpperCheck;

    [SerializeField] private float stepStateDuration = 0.1f; // 계단 타는 상태 유지 시간

    [HideInInspector] public bool isStepping;
    [HideInInspector] public float stepStateTimer;


    public SkillManager skill { get; private set; }
    public GameObject sword {  get ; private set; }
    public PlayerFX fx { get; private set; }


    #region States
    public PlayerStateMachine stateMachine { get; private set; }

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerWallSlideState wallSlide { get; private set; }    
    public PlayerWallJumpState wallJump { get; private set; }
    public PlayerDashState dashState { get; private set; }

    public PlayerPrimaryAttackState primaryAttack { get; private set; }
    public PlayerCounterAttackState counterAttack { get; private set; }

    public PlayerAimSwordState aimSowrd { get; private set; }
    public PlayerCatchSwordState catchSword { get; private set; }
    public PlayerBlackholeState blackHole { get; private set; }
    public PlayerDeadState deadState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState  = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlide = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJump = new PlayerWallJumpState(this, stateMachine, "Jump");

        primaryAttack = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttack = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");

        aimSowrd = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSword = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
        blackHole = new PlayerBlackholeState(this, stateMachine, "Jump");

        deadState = new PlayerDeadState(this, stateMachine, "Die");
    }

    protected override void Start()
    {
        base.Start();

                fx = GetComponent<PlayerFX>();

        skill = SkillManager.instance;

        stateMachine.Initialize(idleState);

        defaultMoveSpeed = moveSpeed;
        defaultJumpForce = jumpForce;
        defaultDashSpeed = dashSpeed;
    }


    protected override void Update()
    {

        if (Time.timeScale == 0)
            return;

        base.Update();

        stateMachine.currentState.Update();

        CheckForDashInput();


        if (Input.GetKeyDown(KeyCode.F) && skill.crystal.crystalUnlocked)
            skill.crystal.CanUseSkill();

        if (Input.GetKeyDown(KeyCode.Alpha1))
            Inventory.instance.UseFlask();
    }

    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        moveSpeed = moveSpeed * (1 - _slowPercentage);
        jumpForce = jumpForce * (1 - _slowPercentage);
        dashSpeed = dashSpeed * (1 - _slowPercentage);
        anim.speed = anim.speed * (1 - _slowPercentage);

        Invoke("ReturnDefaultSpeed", _slowDuration);
        
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;
        jumpForce = defaultJumpForce;
        dashSpeed = defaultDashSpeed;
    }

    public void AssignNewSword(GameObject _newSword)
    {
        sword = _newSword;
    }

    public void CatchTheSword()
    {
        stateMachine.ChangeState(catchSword);
        Destroy(sword);
    }

    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;        

        yield return new WaitForSeconds(_seconds);
        isBusy = false;
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    private void CheckForDashInput()
    {
        if (IsWallDetected())
            return;

        if (skill.dash.dashUnlocked == false)
            return;


        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.instance.dash.CanUseSkill())
        {

            dashDir = Input.GetAxisRaw("Horizontal");

            if (dashDir == 0)
                dashDir = facingDir;

            
            stateMachine.ChangeState(dashState);
        }
    }

    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }

    protected override void SetupZeroKnockbackPower()
    {
        knockbackPower = new Vector2(0, 0);
    }

    public void StepClimb(float moveDir)
    {
        if (Mathf.Abs(moveDir) < 0.01f)
            return;

        Vector2 forward = Vector2.right * moveDir;

        RaycastHit2D lowerHit = Physics2D.Raycast(
            stepLowerCheck.position,
            forward,
            stepCheckDistance,
            whatIsGround
        );

        RaycastHit2D upperHit = Physics2D.Raycast(
            stepUpperCheck.position,
            forward,
            stepCheckDistance,
            whatIsGround
        );

        if (lowerHit && !upperHit)
        {
            // upperCheck 지점에서 아래로 쏘아 실제 '바닥 높이'를 찾는 부분
            RaycastHit2D groundHit = Physics2D.Raycast(
                stepUpperCheck.position + Vector3.up * 0.05f,
                Vector2.down,
                stepHeight + 0.2f,
                whatIsGround
            );

            if (groundHit)
            {
                float targetY = groundHit.point.y + cd.bounds.extents.y;

                Vector2 pos = rb.position;
                pos.y = Mathf.Lerp(pos.y, targetY, stepSmooth * Time.deltaTime);
                rb.position = pos;

                isStepping = true;
                stepStateTimer = stepStateDuration;
            }
        }
    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (stepLowerCheck == null || stepUpperCheck == null)
            return;

        Vector2 forward = Vector2.right * facingDir;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(stepLowerCheck.position, stepLowerCheck.position + (Vector3)forward * stepCheckDistance);
        Gizmos.DrawSphere(stepLowerCheck.position, 0.03f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(stepUpperCheck.position, stepUpperCheck.position + (Vector3)forward * stepCheckDistance);
        Gizmos.DrawSphere(stepUpperCheck.position, 0.03f);
    }
}
