using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float moveSpeed = 5f;

    private bool isMoving = false;

    private CounterTime counter = new CounterTime();

    private bool IsCanUpdate => GameManager.Ins.IsState(GameState.GamePlay) || GameManager.Ins.IsState(GameState.Setting);

    SkinType skinType = SkinType.SKIN_Normal;
    WeaponType weaponType = WeaponType.W_Candy_1;
    HatType hatType = HatType.HAT_Cap;
    AccessoryType accessoryType = AccessoryType.ACC_Headphone;
    PantType pantType = PantType.Pant_1;

    [SerializeField] ParticleSystem reviveVFX;

    public int Coin => Score * 10;

    // Update is called once per frame
    void Update()
    {
        if (IsCanUpdate && !IsDead)
        {

            if (Input.GetMouseButtonDown(0))
            {
                counter.Cancel();
            }

            if (Input.GetMouseButton(0) && JoystickControl.direct != Vector3.zero)
            {
                //rb.velocity = JoystickControl.direct * moveSpeed;
                rb.MovePosition(rb.position + JoystickControl.direct * moveSpeed * Time.deltaTime);
                TF.position = rb.position;

                TF.forward = JoystickControl.direct;

                ChangeAnim(Constant.ANIM_RUN);
                isMoving = true;
            }
            else
            {
                counter.Execute();
            }

            if (Input.GetMouseButtonUp(0))
            {
                isMoving = false;
                OnMoveStop();
                OnAttack();

            }
        }
    }

    public override void OnInit()
    {
        OnTakeClothsData();

        base.OnInit();

        TF.rotation = Quaternion.Euler(Vector3.up * 180);
        SetSize(MIN_SIZE);

        indicator.SetName("YOU");
    }

    public override void WearClothes()
    {
        base.WearClothes(); 

        ChangeSkin(skinType);
        ChangeWeapon(weaponType);
        ChangeHat(hatType);
        ChangeAccessory(accessoryType);
        ChangePant(pantType);
    }

    public override void OnMoveStop()
    {
        base.OnMoveStop();
        rb.velocity = Vector3.zero;
        ChangeAnim(Constant.ANIM_IDLE);
    }

    public override void AddTarget(Character target)
    {
        base.AddTarget(target);

        if (!target.IsDead && !IsDead)
        {
            target.SetMask(true);
            if (!counter.IsRunning && !isMoving)
            {
                OnAttack();
            }
        }
    }

    public override void RemoveTarget(Character target)
    {
        base.RemoveTarget(target);
        target.SetMask(false);
    }

    public override void OnAttack()
    {
        base.OnAttack();
        if (target != null && currentSkin.Weapon.IsCanAttack)
        {
            counter.Start(Throw, TIME_DELAY_THROW);
            ResetAnim();
        }
    }

    protected override void SetSize(float size)
    {
        base.SetSize(size);
        CameraFollower.Ins.SetRateOffset((this.size - MIN_SIZE) / (MAX_SIZE - MIN_SIZE));
    }

    internal void OnRevive()
    {
        ChangeAnim(Constant.ANIM_IDLE);
        IsDead = false;
        ClearTarget();

        reviveVFX.Play();
    }

    public override void OnDeath()
    {
        base.OnDeath();
        counter.Cancel();
    }

    public void TryCloth(UIShop.ShopType shopType, Enum type)
    {
        switch (shopType)
        {
            case UIShop.ShopType.Hat:
                currentSkin.DespawnHat();
                ChangeHat((HatType)type);
                break;

            case UIShop.ShopType.Pant:
                ChangePant((PantType)type);
                break;

            case UIShop.ShopType.Accessory:
                currentSkin.DespawnAccessory();
                ChangeAccessory((AccessoryType)type);
                break;

            case UIShop.ShopType.Skin:
                TakeOffClothes();
                skinType = (SkinType)type;
                WearClothes();
                break;
            case UIShop.ShopType.Weapon:
                currentSkin.DespawnWeapon();
                ChangeWeapon((WeaponType)type);
                break;
            default:
                break;
        }

    }

    //take cloth from data
    internal void OnTakeClothsData()
    {
        // take old cloth data
        skinType = UserData.Ins.playerSkin;
        weaponType = UserData.Ins.playerWeapon;
        hatType = UserData.Ins.playerHat;
        accessoryType = UserData.Ins.playerAccessory;
        pantType = UserData.Ins.playerPant;
    }
}
