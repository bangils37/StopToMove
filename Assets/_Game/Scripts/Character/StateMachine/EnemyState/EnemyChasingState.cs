using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChasingState : IState<CharacterController>
{
    private float chasingTime;

    public void OnEnter(CharacterController t)
    {
        t.isMoving = true;
        t.isAttack = false;

        chasingTime = Random.Range(3f, 5f);

        ((EnemyController)t).AutoMove(LevelManager.Instance.GetPlayerPosition());
        t.ChangeAnim(CharacterController.ANIM_STATE.Run);
    }

    public void OnExecute(CharacterController t)
    {
        chasingTime -= Time.deltaTime;
        if (chasingTime < 0f)
        {
            t.currentState.ChangeState(new EnemyIdleState());
        }

        if (t.HaveTarget())    /// Thêm vào target player khác mà target bình thường khác     
        {
            t.currentState.ChangeState(new EnemyAttackState());
        }
    }

    public void OnExit(CharacterController t)
    {

    }
}
