using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine 
{

    public EnemyState currentState { get; private set; } // 현재 상태

    public void Initialize(EnemyState _startState) // 초기 상태 설정 메서드
    {
        currentState = _startState;
        currentState.Enter();
    }

    public void ChangeState(EnemyState _newState) // 상태 변경 메서드
    {
        currentState.Exit();
        currentState = _newState;
        currentState.Enter();
    }
}
