using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyDieState : IState<CharacterController>
{
    private bool completelyDeath;
    private float timer;

    public void OnEnter(CharacterController t)
    {
        t.isAttack = false;
        t.isMoving = false;
        t.isDeath = true;
        t.ChangeAnim(CharacterController.ANIM_STATE.Die);
        ((EnemyController)t).StopMove();

        completelyDeath = false;
        timer = 1.5f;
    }

    public void OnExecute(CharacterController t)
    {
        if (completelyDeath)
        {
            ((EnemyController)t).OnDespawn();
        }

        timer -= Time.deltaTime;
        if (timer < 0f)
        {
            completelyDeath = true;
        }
    }

    public void OnExit(CharacterController t)
    {
    }
}
