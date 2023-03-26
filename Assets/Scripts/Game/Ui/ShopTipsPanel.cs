using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ShopTipsPanel : BasePanel
{
    int price;
    ShopGoods goods;
    // Start is called before the first frame update
    void Start()
    {

        GetUiContro<Button>("Button_Close").onClick.AddListener(() =>
        {
            UiManager.GetInstance().HidePanel("ShopTipsPanel");
        });
        GetUiContro<Button>("btnClick").onClick.AddListener(() =>
        {
            Click();
        });
    }
    public void Init(ShopGoods goods)
    {
        this.goods= goods;  
        GetUiContro<Text>("name").text = goods.GetUiContro<Text>("name").text;
        GetUiContro<Image>("icon").sprite = goods.GetUiContro<Image>("icon").sprite;
        GetUiContro<Image>("iconType").sprite = goods.GetUiContro<Image>("iconType").sprite;
        GetUiContro<Text>("txtPrice").text = goods.GetUiContro<Text>("txtPrice").text;
        price=int.Parse(GetUiContro<Text>("txtPrice").text);
    }
    protected  void Click()
    {
        if(GetUiContro<Image>("iconType").sprite== ResourcesManager.GetInstance().Load<Sprite>("Ui/Sprite/InventoryPanel/IconGroup_MenuIcon5_Coin"))
        {
            if (price <= TestPlayer.GetInstance().gold)
            {
                GetUiContro<Text>("Text_Info").text = "购买成功！";
                TestPlayer.GetInstance().gold -= price;
                UiManager.GetInstance().GetPanel<MainPanel>("MainPanel").SetMoney();
                UiManager.GetInstance().GetPanel<ShopPanel>("ShopPanel").SetMoney();
                if (TestPlayer.GetInstance().dicItem.ContainsKey(goods.id))
                {
                    TestPlayer.GetInstance().dicItem[goods.id]++;
                }
                else
                {
                    TestPlayer.GetInstance().dicItem.Add(goods.id,1);
                }
            }
            else
            {
                GetUiContro<Text>("Text_Info").text = "金钱不足！";
            }
        }
        else
        {
            if (price <= TestPlayer.GetInstance().gem)
            {
                GetUiContro<Text>("Text_Info").text = "购买成功！";
                TestPlayer.GetInstance().gem -= price;
                UiManager.GetInstance().GetPanel<MainPanel>("MainPanel").SetMoney();
                UiManager.GetInstance().GetPanel<ShopPanel>("ShopPanel").SetMoney();
                if (TestPlayer.GetInstance().dicItem.ContainsKey(goods.id))
                {
                    TestPlayer.GetInstance().dicItem[goods.id]++;
                }
                else
                {
                    TestPlayer.GetInstance().dicItem.Add(goods.id, 1);
                }
            }
            else
            {
                GetUiContro<Text>("Text_Info").text = "宝石不足！";
            }
        }
        
    }
}
