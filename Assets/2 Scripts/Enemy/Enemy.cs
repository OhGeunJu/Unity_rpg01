using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(EntityFX))]
[RequireComponent(typeof(ItemDrop))]
public class Enemy : Entity
{
    [SerializeField] protected LayerMask whatIsPlayer;  // 플레이어 마스크


    [Header("Stunned info")] //
    public float stunDuration = 1; // 넉백 지속 시간
    public Vector2 stunDirection = new Vector2(10,12); // 넉백 방향
    protected bool canBeStunned; // 넉백 가능 여부
    [SerializeField] protected GameObject counterImage; // 넉백 이미지

    [Header("Move info")] // 이동 정보
    public float moveSpeed = 1.5f; // 이동 속도
    public float idleTime = 2; // 대기 시간
    public float battleTime = 7; // 전투 시간
    private float defaultMoveSpeed; // 기본 이동 속도 저장 변수

    [Header("Attack info")] // 공격 정보
    public float agroDistance = 2; // 어그로 거리
    public float attackDistance = 2; // 공격 거리
    public float attackCooldown;
    public float minAttackCooldown = 1;
    public float maxAttackCooldown= 2;
    [HideInInspector] public float lastTimeAttacked; // 마지막 공격 시간

    public EnemyStateMachine stateMachine { get; private set; } // 상태 머신
    public EntityFX fx { get; private set; }
    private Player player;
    public string lastAnimBoolName {  get; private set; } // 마지막 애니메이션 이름
    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine(); // 상태 머신 초기화

        defaultMoveSpeed = moveSpeed; // 기본 이동 속도 저장
    }

    protected override void Start()
    {
        base.Start();

        fx = GetComponent<EntityFX>();
    }

    protected override void Update()
    {
        base.Update();


        stateMachine.currentState.Update(); // 상태 머신의 현재 상태 업데이트


    }

    public virtual void AssignLastAnimName(string _animBoolName) => lastAnimBoolName = _animBoolName; // 마지막 애니메이션 이름 할당 메서드


    public override void SlowEntityBy(float _slowPercentage, float _slowDuration) // 속도 감소 메서드
    {
        moveSpeed = moveSpeed * (1 - _slowPercentage);
        anim.speed = anim.speed * (1 - _slowPercentage);

        Invoke("ReturnDefaultSpeed", _slowDuration);
    }

    protected override void ReturnDefaultSpeed() // 속도 원상복구 메서드
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;
    }

    public virtual void FreezeTime(bool _timeFrozen) // 시간 정지 메서드
    {
        if (_timeFrozen)
        {
            moveSpeed = 0;
            anim.speed = 0;
        }
        else
        {
            moveSpeed = defaultMoveSpeed;
            anim.speed = 1;
        }
    }

    public virtual void FreezeTimeFor(float _duration) => StartCoroutine(FreezeTimerCoroutine(_duration)); // 시간 정지 메서드

    protected virtual IEnumerator FreezeTimerCoroutine(float _seconds) // 시간 정지 코루틴
    {
        FreezeTime(true);

        yield return new WaitForSeconds(_seconds);

        FreezeTime(false);
    }

    #region Counter Attack Window
    public virtual void OpenCounterAttackWindow() // 넉백 창 열기 메서드
    {
        canBeStunned = true;
        counterImage.SetActive(true);
    }

    public virtual void CloseCounterAttackWindow() // 넉백 창 닫기 메서드
    {
        canBeStunned = false;
        counterImage.SetActive(false);
    }
    #endregion

    public virtual bool CanBeStunned() // 넉백 가능 여부 확인 메서드
    {
        if (canBeStunned)
        {
            CloseCounterAttackWindow();
            return true;
        }

        return false;
    }

    public virtual void AnimationFinishTrigger() => stateMachine.currentState.AnimationFinishTrigger(); // 애니메이션 종료 트리거
    public virtual void AnimationSpecialAttackTrigger()  // 특수 공격 트리거
    {

    }

    public virtual RaycastHit2D IsPlayerDetected() // 플레이어 감지 메서드
    {
        RaycastHit2D playerDetected = Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, 50, whatIsPlayer);
        RaycastHit2D wallDetected = Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, 50, whatIsGround);

        if (wallDetected)
        {
            if (wallDetected.distance < playerDetected.distance)
                return default(RaycastHit2D);
        }

        return playerDetected;
    }
    protected override void OnDrawGizmos() // 에디터에서 시각적으로 감지 범위를 표시 (디버깅용)
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + attackDistance * facingDir, transform.position.y)); // 공격 거리
    }
}
