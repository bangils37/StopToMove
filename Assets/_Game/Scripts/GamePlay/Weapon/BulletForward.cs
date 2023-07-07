using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletForward : Bullet
{
    public const float TIME_ALIVE = 1f;

    CounterTime counterTime = new CounterTime();

    public override void OnInit(Character character, Vector3 target, float size)
    {
        base.OnInit(character, target, size);
        counterTime.Start(OnDespawn, TIME_ALIVE * size);
    }

    // Update is called once per frame
    void Update()
    {
        counterTime.Execute();
        if (isRunning)
        {
            TF.Translate(TF.forward * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    protected override void OnStop()
    {
        base.OnStop();
        isRunning = false;
    }
}
