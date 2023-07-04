using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : IState<CharacterController>
{
    private float delayAttackTime;
    private bool hadAttacked;
    private float timer;
    public void OnEnter(CharacterController t)
    {
        t.isMoving = false;
        t.isAttack = true;

        hadAttacked = false;
        delayAttackTime = 0.5f;
        timer = 1.5f;

        ((EnemyController)t).StopMove();
    }

    public void OnExecute(CharacterController t)
    {
        timer -= Time.deltaTime;
        if(!t.isAttack || timer < 0f)
        {
            t.currentState.ChangeState(new EnemyIdleState());
        }

        delayAttackTime -= Time.deltaTime;
        if(delayAttackTime < 0f && !hadAttacked)
        {
            hadAttacked = true;
            t.Attack();
        }
    }

    public void OnExit(CharacterController t)
    {
        t.isAttack = false;
    }
}
