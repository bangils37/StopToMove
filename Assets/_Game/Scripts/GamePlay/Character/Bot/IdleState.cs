using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState<Bot>
{
    public void OnEnter(Bot t)
    {
        t.OnMoveStop();
        t.Counter.Start(() => t.ChangeState(new PatrolState()), Random.Range(0f, 2f));
    }

    public void OnExecute(Bot t)
    {
        t.Counter.Execute();
    }

    public void OnExit(Bot t)
    {

    }

}
