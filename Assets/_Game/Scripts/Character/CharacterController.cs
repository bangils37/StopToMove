using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#region Task
/*
    //- Điều chỉnh scale của weapon
    //- Điều chỉnh độ scale của character
        //+ Những level đầu thì scale to nhanh, level sau thì giảm dần độ to
        //+ Scale của camera follow vừa phải
    //- Điều chỉnh độ lớn của tốc độ khi tăng level
    //- Spawn Enemy theo level của Player
    - Pooling weapon
    - Bot đánh xong chạy
    //- Thêm ném vũ khí trúng character là hủy
    //- Thêm ném vũ khí không đi xuyên tường
    //- Thêm ném vũ khí đi hết 1 bán kính target controller
    //- Scale từ từ
    //- Thêm State đuổi Player
    //    + Khi idle vẫn tấn công những enemy khác 
    //    + Khi run thì run hết mình tới player 
*/
#endregion

public class CharacterController : GameUnit
{
    public enum ANIM_STATE
    {
        Idle,
        Run,
        Attack,
        Win,
        Die
    }

    [SerializeField] private Animator characterAnimator;

    [SerializeField] private SkinnedMeshRenderer characterMesh;
    [SerializeField] private SkinnedMeshRenderer pantMesh;
    [SerializeField] private List<Material> listClothesMaterials;

    [SerializeField] private Transform weaponTransform;
    [SerializeField] private Transform weaponBase;
    [SerializeField] private GameObject hammerPrefab;

    [SerializeField] private GameObject aimRing;

    [SerializeField] protected TargetController targetController;

    [SerializeField] protected LayerMask characterMask;

    [SerializeField] protected float ratioScale;
    private Vector3 initScaleCharacter;
    [SerializeField] protected Vector3 scaleCharacter;

    private float initSpeedCharacter;
    [SerializeField] protected float speedCharacter;

    public int characterLevel;
    public int countdownRunToRandomTarget;

    public bool isDeath;
    public bool isAttack;
    public bool isMoving;

    protected Collider[] listEnemyAround;


    public override void OnInit()
    {
        isDeath = false;
        isAttack = false;
        isMoving = false;

        ratioScale = 1;
        characterLevel = 1;
        initScaleCharacter = new Vector3(1, 1, 1);
        scaleCharacter = initScaleCharacter;

        initSpeedCharacter = 6;
        speedCharacter = initSpeedCharacter;

        targetController.ClearAllTarget();
        SetClothes(Random.Range(0,9));
    }

    public override void OnInit(CharacterController t)
    {
        throw new System.NotImplementedException();
    }

    public override void OnInit(Vector3 spawnPosition, Vector3 targetEnemy)
    {
        throw new System.NotImplementedException();
    }

    public override void OnDespawn()
    {
        SimplePool.Despawn(this);
    }

    public void SetClothes(int clothesId)
    {
        if(clothesId < listClothesMaterials.Count)
        {
            characterMesh.material = listClothesMaterials[clothesId];
            pantMesh.material = listClothesMaterials[clothesId];
        }
    }

    public void SetActiveRing(bool active)
    {
        aimRing.SetActive(active);
    }

    internal void SetNewCountdownRunToRandomTarget()
    {
        countdownRunToRandomTarget = UnityEngine.Random.Range(0, 5);
    }

    public void RandomPickTarget(ref Vector3 target)
    {
        listEnemyAround = Physics.OverlapSphere(this.transform.position, 100f, characterMask);      /// Thêm bán kinh để những thằng to có thể tìm kiếm ở phạm vi tốt hơn, tối ưu hơn
                                                                                                    /// Không sợ đứng yên 1 chỗ vì mình có state lùa player, hoặc đứng yên cũng được

        if (listEnemyAround.Length < 1)
            return;

        float distance = 1000000f;
        float temp;

        for (int i = 0; i < listEnemyAround.Length; i++)
        {
            temp = Vector3.Distance(transform.position, listEnemyAround[i].transform.position);
            if (temp < 0.001f)
                continue;
            if (distance > temp)
            {
                distance = temp;
                target = listEnemyAround[i].transform.position;
            }
        }
    }

    #region ChangeLevel

    public virtual void AddLevel(int level)
    {
        characterLevel += level;
        ratioScale = GetSigmoidFunction(characterLevel);
        ChangeScale(ratioScale);
        ChangeSpeed(ratioScale);

    }

    public void ChangeScale(float ratio)
    {
        scaleCharacter = initScaleCharacter * ratio;
        transform.localScale = scaleCharacter;
    }

    public void ChangeSpeed(float ratio)
    {
        speedCharacter = initSpeedCharacter * ratio;
    }

    public float GetSigmoidFunction(float X)
    {
        return 1.2f / (0.5f + Mathf.Pow(1.5f, -X));  
    }    

    #endregion

    #region StateMachine

    public StateMachine<CharacterController> currentState;

    private void Awake()
    {
        currentState = new StateMachine<CharacterController>();
        currentState.SetOwner(this);
    }

    #endregion


    #region Anim

    public ANIM_STATE currentAnim = ANIM_STATE.Idle;

    public void ChangeAnim(ANIM_STATE _newState)
    {
        if(currentAnim != _newState)
        {
            currentAnim = _newState;
            characterAnimator.SetTrigger(currentAnim.ToString());
        }
    }

    #endregion


    #region Attack

    public bool HaveTarget()
    {
        GameObject enemy = targetController.FindTheTarget();
        return enemy != null;
    }

    public void Attack()
    {
        GameObject enemy = targetController.FindTheTarget();

        if (enemy == null)
        {
            return;
        }
        isAttack = true;

        transform.rotation = Quaternion.LookRotation(enemy.transform.position - transform.position, Vector3.up);
        ChangeAnim(ANIM_STATE.Attack);
    }

    public void ThrowWeapon()
    {
        GameObject enemy = targetController.FindTheTarget();

        if (enemy != null)
        {
            GameObject weaponObject = GameObject.Instantiate(hammerPrefab);
            WeaponController weapon = weaponObject.GetComponent<WeaponController>();
            weaponObject.transform.position = weaponBase.transform.position;
            weaponObject.transform.rotation = weaponBase.transform.rotation;

            weapon.OnInit();
            weapon.SetOwner(this);
            weapon.ChangeWeaponScale(ratioScale);
            weapon.Shoot(enemy.transform);
        }
        weaponTransform.gameObject.SetActive(false);
    }

    public void EndAttack()
    {
        isAttack = false;
        weaponTransform.gameObject.SetActive(true);

        transform.rotation = new Quaternion(0, 0, 0, 0);
        ChangeAnim(ANIM_STATE.Idle);
    }

    #endregion
}
