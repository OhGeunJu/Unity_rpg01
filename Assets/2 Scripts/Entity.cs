using System.Collections;
using UnityEngine;


public class Entity : MonoBehaviour
{

    #region Components
    // 초기화 
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    
    public SpriteRenderer sr { get; private set; }
    public CharacterStats stats { get; private set; }
    public CapsuleCollider2D cd {  get; private set; }
    #endregion


    [Header("Collision info")]
    public Transform attackCheck; // 공격 범위 체크 위치
    public float attackCheckRadius = 1.2f;
    [SerializeField] protected Transform groundCheck; // 땅 체크 위치
    [SerializeField] protected float groundCheckDistance = 1;
    [SerializeField] protected Transform wallCheck; // 벽 체크 위치
    [SerializeField] protected float wallCheckDistance = .8f;
    [SerializeField] protected LayerMask whatIsGround;  // 땅 레이어 마스크

    [Header("Knockback info")]
    [SerializeField] protected Vector2 knockbackPower = new Vector2(7,12); // knockbackPower : 피격 시 캐릭터가 튕겨나가는 힘 (x, y 방향)
    [SerializeField] protected Vector2 knockbackOffset = new Vector2(.5f,2);
    [SerializeField] protected float knockbackDuration = .07f;
    protected bool isKnocked;
    public bool IsKnocked => isKnocked;
    public int knockbackDir { get; private set; } // knockbackDir : 맞은 방향 반대쪽으로 밀려나도록 방향 설정

    public int facingDir { get; private set; } = 1;
    protected bool facingRight = true;

    public System.Action onFlipped;

    protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        stats = GetComponent<CharacterStats>();
        cd = GetComponent<CapsuleCollider2D>();
    }

    protected virtual void Update()
    {

    }

    public virtual void SlowEntityBy(float _slowPercentage, float _slowDuration)  // 속도 감소 메서드
    {
        
    }

    protected virtual void ReturnDefaultSpeed() // 속도 원상복구 메서드
    {
        anim.speed = 1;
    }

    // 피격 시 넉백 코루틴 실행
    public virtual void DamageImpact() => StartCoroutine("HitKnockback");
 
    public virtual void SetupKnockbackDir(Transform _damageDirection)
    {
        if (_damageDirection.position.x > transform.position.x)
            knockbackDir = -1;
        else if (_damageDirection.position.x < transform.position.x)
            knockbackDir = 1;

       
    }

    public void SetupKnockbackPower(Vector2 _knockbackpower) => knockbackPower = _knockbackpower;  // 넉백 파워 설정 메서드
    protected virtual IEnumerator HitKnockback()  // 넉백 코루틴
    {
        isKnocked = true;

        float xOffset = Random.Range(knockbackOffset.x, knockbackOffset.y);


        if(knockbackPower.x > 0 || knockbackPower.y > 0) // This line makes player immune to freeze effect when he takes hit
            rb.velocity = new Vector2((knockbackPower.x + xOffset) * knockbackDir, knockbackPower.y);

        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;
        SetupZeroKnockbackPower();
    }

    protected virtual void SetupZeroKnockbackPower() // 넉백 파워를 0으로 설정
    {

    }

    #region Velocity
    public void SetZeroVelocity()  // 속도를 0으로 설정하는 메서드
    {
        if (isKnocked)
            return;

        rb.velocity = new Vector2(0, 0);
    }

    public void SetVelocity(float _xVelocity, float _yVelocity)  // 지정된 속도로 설정하는 메서드
    {
        if (isKnocked)
            return;

        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }
    #endregion
    #region Collision
    public virtual bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    public virtual bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);

    protected virtual void OnDrawGizmos() // 에디터에서 시각적으로 감지 범위를 표시 (디버깅용)
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance * facingDir, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
    }
    #endregion
    #region Flip
    public virtual void Flip() // 캐릭터의 방향을 반전시키는 메서드
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);

        if(onFlipped != null)
            onFlipped();
    }

    public virtual void FlipController(float _x) // 이동 방향에 따라 캐릭터의 방향을 자동으로 조정하는 메서드
    {
        if (_x > 0 && !facingRight)
            Flip();
        else if (_x < 0 && facingRight)
            Flip();
    }

    public virtual void SetupDefailtFacingDir(int _direction)  // 초기 방향 설정 메서드
    {
        facingDir = _direction;

        if (facingDir == -1)
            facingRight = false;
    }
    #endregion

    

    public virtual void Die()
    {

    }
}
