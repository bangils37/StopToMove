using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : UICanvas
{
    public enum ShopType { Hat, Pant, Accessory, Skin, Weapon }

    [SerializeField] ShopData data;
    [SerializeField] ShopItem prefab;
    [SerializeField] Transform content;
    [SerializeField] ShopBar[] shopBars;

    [SerializeField] TextMeshProUGUI playerCoinTxt;

    [SerializeField] ButtonState buttonState;
    [SerializeField] Text coinTxt;
    [SerializeField] Text adsTxt;

    MiniPool<ShopItem> miniPool = new MiniPool<ShopItem>();

    private ShopItem currentItem;
    private ShopBar currentBar;

    private ShopItem itemEquiped;

    public ShopType shopType => currentBar.Type;

    private void Awake()
    {
        miniPool.OnInit(prefab, 10, content);

        for (int i = 0; i < shopBars.Length; i++)
        {
            shopBars[i].SetShop(this);
        }
    }

    public override void Open()
    {
        base.Open();
        SelectBar(shopBars[0]);
        CameraFollower.Ins.ChangeState(CameraFollower.State.Shop);

        playerCoinTxt.SetText(UserData.Ins.coin.ToString());
    }

    public override void CloseDirectly()
    {
        base.CloseDirectly();
        UIManager.Ins.OpenUI<UIMainMenu>();

        LevelManager.Ins.player.TakeOffClothes();
        LevelManager.Ins.player.OnTakeClothsData();
        LevelManager.Ins.player.WearClothes();
    }

    internal void SelectBar(ShopBar shopBar)
    {
        if (currentBar != null)
        {
            currentBar.Active(false);
        }

        currentBar = shopBar;
        currentBar.Active(true);

        miniPool.Collect();
        itemEquiped = null;

        switch (currentBar.Type)
        {
            case ShopType.Hat:
                InitShipItems(data.hats.Ts, ref itemEquiped);
                break;
            case ShopType.Pant:
                InitShipItems(data.pants.Ts, ref itemEquiped);
                break;
            case ShopType.Accessory:
                InitShipItems(data.accessories.Ts, ref itemEquiped);
                break;
            case ShopType.Skin:
                InitShipItems(data.skins.Ts, ref itemEquiped);
                break;
            default:
                break;
        }

    }

    //public string GetDataKeys(ShopType shopType)
    //{
    //    switch (shopType)
    //    {
    //        case ShopType.Hat: return UserData.Keys_Hat_Data;
    //        case ShopType.Pant: return UserData.Keys_Pant_Data;
    //        case ShopType.Accessory: return UserData.Keys_Accessory_Data;
    //        case ShopType.Skin: return UserData.Keys_Skin_Data;
    //        case ShopType.Weapon: return UserData.Keys_Weapon_Data;
    //        default: return string.Empty;
    //    }
    //}  
    
    //public List<ShopItemData<T>> GetListItems<T>(ShopType shopType) where T : Enum
    //{
    //    switch (shopType)
    //    {
    //        case ShopType.Hat: return data.hats.Ts as List<ShopItemData<T>>;
    //        case ShopType.Pant: return data.pants.Ts as List<ShopItemData<T>>;
    //        case ShopType.Accessory: return data.accessories.Ts as List<ShopItemData<T>>;
    //        case ShopType.Skin: return data.skins.Ts as List<ShopItemData<T>>;
    //        case ShopType.Weapon: return null;
    //        default: return null;
    //    }
    //}


    private void InitShipItems<T>(List<ShopItemData<T>> items, ref ShopItem itemEquiped)  where T : Enum
    {
        for (int i = 0; i < items.Count; i++)
        {
            ShopItem.State state = UserData.Ins.GetEnumData(items[i].type.ToString(), ShopItem.State.Buy);
            ShopItem item = miniPool.Spawn();
            item.SetData(i, items[i], this);
            item.SetState(state);
            
            if (state == ShopItem.State.Equipped)
            {
                SelectItem(item);
                itemEquiped = item;
            }

        }
    }

    public ShopItem.State GetState(Enum t) => UserData.Ins.GetEnumData(t.ToString(), ShopItem.State.Buy);

    internal void SelectItem(ShopItem item)
    {
        if (currentItem != null)
        {
            currentItem.SetState(GetState(currentItem.Type));
        }

        currentItem = item;

        switch (currentItem.state)
        {
            case ShopItem.State.Buy:
                buttonState.SetState(ButtonState.State.Buy);
                break;
            case ShopItem.State.Bought:
                buttonState.SetState(ButtonState.State.Equip);
                break;
            case ShopItem.State.Equipped:
                buttonState.SetState(ButtonState.State.Equiped);
                break;
            case ShopItem.State.Selecting:
                break;
            default:
                break;
        }

        LevelManager.Ins.player.TryCloth(shopType, currentItem.Type);
        currentItem.SetState(ShopItem.State.Selecting);

        //check data
        coinTxt.text = item.data.cost.ToString();
        adsTxt.text = item.data.ads.ToString();
    }

    public void BuyButton()
    {
        //TODO: check xem du tien hay k

        UserData.Ins.SetEnumData(currentItem.Type.ToString(), ShopItem.State.Bought);
        SelectItem(currentItem);
    }

    public void AdsButton()
    {

    }

    public void EquipButton()
    {
        if (currentItem != null)
        {
            UserData.Ins.SetEnumData(currentItem.Type.ToString(), ShopItem.State.Equipped);

            switch (shopType)
            {
                case ShopType.Hat:
                    //reset trang thai do dang deo ve bought
                    UserData.Ins.SetEnumData(UserData.Ins.playerHat.ToString(), ShopItem.State.Bought);
                    //save id do moi vao player
                    UserData.Ins.SetEnumData(UserData.Key_Player_Hat, ref UserData.Ins.playerHat, (HatType)currentItem.Type);
                    break;
                case ShopType.Pant:
                    UserData.Ins.SetEnumData(UserData.Ins.playerPant.ToString(), ShopItem.State.Bought);
                    UserData.Ins.SetEnumData(UserData.Key_Player_Pant, ref UserData.Ins.playerPant, (PantType)currentItem.Type);
                    break;
                case ShopType.Accessory:
                    UserData.Ins.SetEnumData(UserData.Ins.playerAccessory.ToString(), ShopItem.State.Bought);
                    UserData.Ins.SetEnumData(UserData.Key_Player_Accessory, ref UserData.Ins.playerAccessory, (AccessoryType)currentItem.Type);
                    break;
                case ShopType.Skin:
                    UserData.Ins.SetEnumData(UserData.Ins.playerSkin.ToString(), ShopItem.State.Bought);
                    UserData.Ins.SetEnumData(UserData.Key_Player_Skin, ref UserData.Ins.playerSkin, (SkinType)currentItem.Type);
                    break;
                case ShopType.Weapon:
                    break;
                default:
                    break;
            }
  
        }

        if (itemEquiped != null)
        {
            itemEquiped.SetState(ShopItem.State.Bought);
        }

        currentItem.SetState(ShopItem.State.Equipped);
        SelectItem(currentItem);
    }

}
