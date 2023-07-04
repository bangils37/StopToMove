using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterController
{
    [SerializeField] private VariableJoystick variableJoystick;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CameraFollow camera;

    private void Start()
    {
        OnInit();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (isAttack || isDeath)
        {
            return;
        }

        if (Mathf.Abs(variableJoystick.Vertical) >= 0.1f || Mathf.Abs(variableJoystick.Horizontal) >= 0.1f)
        {
            ChangeAnim(ANIM_STATE.Run);

            Vector3 direction = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;
            rb.MovePosition(transform.position + (direction * speedCharacter * Time.fixedDeltaTime));

            transform.rotation = Quaternion.LookRotation(direction);
        }
        else
        {
            if (!isAttack && HaveTarget())
            {
                Attack();
            }
            
            ChangeAnim(ANIM_STATE.Idle);
        }
    }

    public override void AddLevel(int level)
    {
        base.AddLevel(level);
        camera.ChangeOffsetCamera(ratioScale);
    }

    public override void OnInit()
    {
        base.OnInit();
    }

    public override void OnDespawn()
    {

    }
}

