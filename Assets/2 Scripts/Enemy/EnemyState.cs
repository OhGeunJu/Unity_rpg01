using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyState 
{

    protected EnemyStateMachine stateMachine; // 상태 전환을 관리하는 컨트롤러
    protected Enemy enemyBase; // 상태를 사용하는 적 캐릭터
    protected Rigidbody2D rb;

    private string animBoolName;

    protected float stateTimer; // 상태 지속 시간 타이머
    protected bool triggerCalled; // 애니메이션 트리거 호출 여부

    public EnemyState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName) // 상태 생성 시 Enemy, EnemyStateMachine, animBoolName을 전달받습니다.
    {
        this.enemyBase = _enemyBase;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime; // 상태 지속 시간 타이머 감소
    }


    public virtual void Enter()
    {
        triggerCalled = false;
        rb = enemyBase.rb;
        enemyBase.anim.SetBool(animBoolName, true); // 상태에 해당하는 애니메이션 활성화

    }

    public virtual void Exit()
    {
        enemyBase.anim.SetBool(animBoolName, false); // 상태에 해당하는 애니메이션 비활성화
        enemyBase.AssignLastAnimName(animBoolName); // Enemy에 현재 상태의 애니메이션 이름 저장
    }

    public virtual void AnimationFinishTrigger() // 애니메이션 종료 트리거 메서드
    {
        triggerCalled = true; // 하위 상태(EnemyAttackState 등)에서 if (triggerCalled) stateMachine.ChangeState(nextState); 식으로 상태 전환에 사용됩니다.
    }
}
