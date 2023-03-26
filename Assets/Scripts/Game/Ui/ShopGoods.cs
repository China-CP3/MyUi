using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopGoods : BasePanel
{
    public int id;
    // Start is called before the first frame update
    void Start()
    {
        GetUiContro<Button>("btnBuy").onClick.AddListener(() =>
        {
            UiManager.GetInstance().ShowPanel<ShopTipsPanel>("ShopTipsPanel", E_Ui_Layer.Tips, (panel) =>
            {
                panel.Init(this);
            });
        });
    }

    public void Init(int id)
    {     
        var item = ExcelReaderManager.GetInstance().GetTable<Item>().dataDic[id];
        this.id = id;
        GetUiContro<Text>("name").text = item.name;
        GetUiContro<Image>("icon").sprite = ResourcesManager.GetInstance().Load<Sprite>(item.icon);
        GetUiContro<Image>("iconType").sprite = item.buyType==1? ResourcesManager.GetInstance().Load<Sprite>("Ui/Sprite/InventoryPanel/IconGroup_MenuIcon5_Coin") : ResourcesManager.GetInstance().Load<Sprite>("Ui/Sprite/InventoryPanel/IconGroup_MenuIcon5_Gem");
        GetUiContro<Text>("txtPrice").text = item.sale != 0? (item.price * item.sale).ToString() :item.price.ToString();
        GetUiContro<Text>("saleText").text = item.sale + "уш";
        GetUiContro<Image>("sale").gameObject.SetActive(item.sale!=0);
    }    
}
