using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : IState<CharacterController>
{
    private float stopTime;

    public void OnEnter(CharacterController t)
    {
        t.isMoving = false;
        t.isAttack = false;
        stopTime = Random.Range(0f, 3f);

        ((EnemyController)t).StopMove();
        t.ChangeAnim(CharacterController.ANIM_STATE.Idle);
    }

    public void OnExecute(CharacterController t)
    {
        if(t.HaveTarget())
        {
            t.currentState.ChangeState(new EnemyAttackState());
            return;
        }

        stopTime -= Time.deltaTime;
        if (stopTime < 0f)
        {
            t.currentState.ChangeState(new EnemyRunState());
            return;
        }
    }

    public void OnExit(CharacterController t)
    {
    }
}
