using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public VariableJoystick variableJoystick;
    public Rigidbody rb;
    public CharacterController characterObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Update()
    {
        if(characterObject.isAttack || characterObject.isDeath)
        {
            return;
        }

        if(Mathf.Abs(variableJoystick.Vertical) >= 0.1f || Mathf.Abs(variableJoystick.Horizontal) >=0.1f)
        {
            characterObject.ChangeAnim(CharacterController.ANIM_STATE.Run);

            Vector3 direction = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;
            transform.position = new Vector3(this.transform.position.x + variableJoystick.Horizontal * speed * Time.deltaTime,
                                             this.transform.position.y,
                                             this.transform.position.z + variableJoystick.Vertical * speed * Time.deltaTime);

            transform.rotation = Quaternion.LookRotation(direction);
        }
        else 
        {
            characterObject.ChangeAnim(CharacterController.ANIM_STATE.Idle);
        }
    }
}
