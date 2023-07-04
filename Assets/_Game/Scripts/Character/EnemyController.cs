using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : CharacterController
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private NavMeshAgent agent;


    void Start()
    {
        agent.speed = speedCharacter;
    }

    public override void OnInit()
    {
        base.OnInit();

        SetActiveRing(false);
        SetNewCountdownRunToRandomTarget();
        LevelManager.Instance.EnemyInit(this);
        currentState.ChangeState(new EnemyIdleState());
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        LevelManager.Instance.EnemyDespawn(this);
    }

    void Update()
    {
        currentState.UpdateState();
    }

    #region EnemyActivitis
    public bool PlayerInTarget()
    {
        return (targetController.FindTheTarget().GetComponent<CharacterController>()) is PlayerController;
    }    

    public void StopMove()
    {
        agent.enabled = false;
        ChangeAnim(ANIM_STATE.Idle);
    }

    public void AutoMove(Vector3 target)
    {
        transform.rotation = Quaternion.LookRotation(target - transform.position);
        agent.enabled = true;
        agent.SetDestination(target);   
    }
    #endregion
}
