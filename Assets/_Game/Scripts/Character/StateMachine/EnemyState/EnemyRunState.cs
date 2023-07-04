using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRunState : IState<CharacterController>
{
    private float patrolTime;
    private Vector3 targetPosition;

    public void OnEnter(CharacterController t)
    {
        t.isMoving = true;
        t.isAttack = false;

        targetPosition = new Vector3();
        patrolTime = Random.Range(2f, 5f);
        if (t.countdownRunToRandomTarget < -1)     /// Luôn có ít nhất là 2 lần Enemy truy tìm Player vào lúc biến countdownRunToRandomTarget = 0 và = -1
        {
            t.SetNewCountdownRunToRandomTarget();
        }

        if (t.countdownRunToRandomTarget > 0)
        {
            t.RandomPickTarget(ref targetPosition);
        }
        else
        {
            t.currentState.ChangeState(new EnemyChasingState());
        }

        if(targetPosition != null)
        {
            ((EnemyController)t).AutoMove(targetPosition);
            t.ChangeAnim(CharacterController.ANIM_STATE.Run);
        }
    }

    public void OnExecute(CharacterController t)
    {
        patrolTime -= Time.deltaTime;
        if (patrolTime < 0f || targetPosition == null)
        {
            t.currentState.ChangeState(new EnemyIdleState());
        }   

        if (t.HaveTarget())    
        {
            t.currentState.ChangeState(new EnemyAttackState());
        }
    }

    public void OnExit(CharacterController t)
    {
        t.countdownRunToRandomTarget--;
    }
}
