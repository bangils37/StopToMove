using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T> where T : CharacterController
{
    private IState<T> currentState;
    private T typeClass;

    public void ChangeState<TState>(TState newState) where TState : IState<T>
    {
        if (currentState != null)
        {
            currentState.OnExit(typeClass);
        }

        currentState = newState;

        if (currentState != null)
        {
            currentState.OnEnter(typeClass);
        }
    }

    public void UpdateState()
    {
        if(currentState != null)
        {
            currentState.OnExecute(typeClass);
        }
    }

    public void SetOwner(T owner)
    {
        typeClass = owner;
    }

    public void DebugState()
    {
        Debug.Log(currentState.ToString());
    }
}
