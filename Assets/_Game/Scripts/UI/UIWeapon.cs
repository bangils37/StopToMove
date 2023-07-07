using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UI;

public class UIWeapon : UICanvas
{
    public Transform weaponPoint;

    [SerializeField] WeaponData weaponData;
    [SerializeField] ButtonState buttonState;
    [SerializeField] TextMeshProUGUI nameTxt;
    [SerializeField] TextMeshProUGUI playerCoinTxt;
    [SerializeField] Text coinTxt;
    [SerializeField] Text adsTxt;

    private Weapon currentWeapon;
    private WeaponType weaponType;

    public override void Setup()
    {
        base.Setup();
        ChangeWeapon(UserData.Ins.playerWeapon);
        playerCoinTxt.SetText(UserData.Ins.coin.ToString());
    }

    public override void CloseDirectly()
    {
        base.CloseDirectly();

        if (currentWeapon != null)
        {
            SimplePool.Despawn(currentWeapon);
            currentWeapon = null;
        }

        UIManager.Ins.OpenUI<UIMainMenu>();
    }

    public void NextButton()
    {
        ChangeWeapon(weaponData.NextType(weaponType));
    }

    public void PrevButton()
    {
        ChangeWeapon(weaponData.PrevType(weaponType));
    }

    public void BuyButton()
    {
        //TODO: check tien
        if (true)
        {
            UserData.Ins.SetEnumData(weaponType.ToString(), ShopItem.State.Bought);
            ChangeWeapon(weaponType);
        }
    }

    public void AdsButton()
    {
        int ads = UserData.Ins.GetDataState(weaponType + "Ads", 0);
        UserData.Ins.SetDataState(weaponType + "Ads", ads + 1);

        if (ads + 1 >= weaponData.GetWeaponItem(weaponType).ads)
        {
            UserData.Ins.SetDataState(weaponType.ToString(), 1);
            ChangeWeapon(weaponType);
        }
    }

    public void EquipButton()
    {
        UserData.Ins.SetEnumData(weaponType.ToString(), ShopItem.State.Equipped);
        UserData.Ins.SetEnumData(UserData.Ins.playerWeapon.ToString(), ShopItem.State.Bought);
        UserData.Ins.SetEnumData(UserData.Key_Player_Weapon, ref UserData.Ins.playerWeapon, weaponType);
        ChangeWeapon(weaponType);
        LevelManager.Ins.player.TryCloth( UIShop.ShopType.Weapon, weaponType);
    }

    public void ChangeWeapon(WeaponType weaponType)
    {
        this.weaponType = weaponType;

        if (currentWeapon != null )
        {
            SimplePool.Despawn(currentWeapon);
        }
        currentWeapon = SimplePool.Spawn<Weapon>((PoolType)weaponType, Vector3.zero, Quaternion.identity, weaponPoint);

        //check data dong
        ButtonState.State state = (ButtonState.State)UserData.Ins.GetDataState(weaponType.ToString(), 0);
        buttonState.SetState(state);

        WeaponItem item = weaponData.GetWeaponItem(weaponType);
        nameTxt.SetText(item.name);
        coinTxt.text = item.cost.ToString();
        adsTxt.text = item.ads.ToString();
    }

}
