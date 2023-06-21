using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public enum ANIM_STATE
    {
        Idle,
        Run,
        Attack,
        Win,
        Death
    }

    [SerializeField] GameObject human;
    [SerializeField] Animator characterAnimator;
    [SerializeField] SkinnedMeshRenderer characterMesh;
    [SerializeField] SkinnedMeshRenderer pantMesh;
    [SerializeField] Transform weaponTransform;
    [SerializeField] Transform weaponBase;
    [SerializeField] TargetController targetController;

    [SerializeField] GameObject hammerPrefab;
    public List<Material> listClothesMaterials;

    public bool isDeath = false;
    public bool isAttack = false;
    public float attack_time = 0;
    public ANIM_STATE currentAnimState = ANIM_STATE.Idle;
    // Start is called before the first frame update
    void Start()
    {
        SetClothes(Random.Range(0,9));
    }

    // Update is called once per frame
    void Update()
    {
        if(isDeath) 
        { 
            return;
        }
        
        attack_time -= Time.deltaTime;
        if (attack_time <= 0)
        {
            Attack();
        }
    }

    public void ChangeAnim(ANIM_STATE _newState)
    {
        if(currentAnimState != _newState)
        {
            currentAnimState = _newState;
            characterAnimator.SetTrigger(currentAnimState.ToString());
        }
    }

    public void SetClothes(int clothesId)
    {
        if(clothesId < listClothesMaterials.Count)
        {
            characterMesh.material = listClothesMaterials[clothesId];
            pantMesh.material = listClothesMaterials[clothesId];

        }
    }

    public void Attack()
    {
        if(targetController != null && targetController.listEnemy.Count>0)
        {
            Debug.Log(targetController);
            isAttack = true;
            attack_time = 3f;
            ChangeAnim(ANIM_STATE.Attack);
        }
    }

    public void ThrowWeapon()
    {
        if(targetController.FindTheTarget() != null)
        {
            GameObject weaponObject = GameObject.Instantiate(hammerPrefab);
            weaponObject.transform.position = weaponBase.transform.position;
            weaponObject.transform.rotation = weaponBase.transform.rotation;

            human.transform.rotation = Quaternion.LookRotation(targetController.FindTheTarget().transform.position - transform.position);
            weaponObject.GetComponent<WeaponController>().Shoot(targetController.FindTheTarget().transform);
        }
        weaponTransform.gameObject.SetActive(false);
    }

    public void EndAttack()
    {
        isAttack = false;
        weaponTransform.gameObject.SetActive(true);
        ChangeAnim(ANIM_STATE.Idle);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Weapon")
        {
            ChangeAnim(ANIM_STATE.Death);
            isDeath = true;
        }
    }

}
