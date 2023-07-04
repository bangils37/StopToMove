using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WeaponController : GameUnit
{
    [SerializeField] private float speedWeapon;
    [SerializeField] private bool shotting;
    [SerializeField] private LayerMask characterLayerMask;
    [SerializeField] private float timer;
    private Vector3 direction;
    private Vector3 scaleWeapon;
    private Vector3 initScaleWeapon;

    private Vector3 targetPos;
    private CharacterController owner;

    private void Start()
    {
        OnInit();
    }

    public override void OnDespawn()
    {
        throw new System.NotImplementedException();
    }

    public override void OnInit()
    {
        initScaleWeapon = transform.localScale;
        speedWeapon = 1f;
        timer = 1.2f;
    }

    public override void OnInit(CharacterController t)
    {
        throw new System.NotImplementedException();
    }

    public override void OnInit(Vector3 spawnPosition, Vector3 targetEnemy)
    {
        throw new System.NotImplementedException();
    }

    public void SetOwner(CharacterController owner)
    {
        this.owner = owner;
    }

    public void ChangeWeaponScale(float ratio)
    {
        scaleWeapon = initScaleWeapon * ratio;
        transform.localScale = scaleWeapon;
        timer = 1.2f * ratio;
    }

    public void Shoot(Transform _target) 
    {
        targetPos = new Vector3(_target.position.x, this.transform.position.y, _target.position.z);
        direction = targetPos - this.transform.position;
        shotting = true;
        //RaycastHit hit;
        //if (Physics.Raycast(transform.position, direction, out hit, 100f, characterLayerMask))
        //{
        //    Debug.DrawLine(transform.position, hit.point, Color.white);
        //}
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (shotting)
        {
            transform.Translate(direction * speedWeapon * Time.deltaTime, Space.World);
            transform.Rotate(0, 0, 1200f * Time.deltaTime);
        }
        if (timer < 0f)
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (!enemy.isDeath)
            {
                enemy.currentState.ChangeState(new EnemyDieState());
                owner.AddLevel(1);
                GameObject.Destroy(this.gameObject);
            }    
        }
    }
}
