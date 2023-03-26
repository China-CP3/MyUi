using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : BasePanel
{
    public enum E_GoodsType
    {
        All,
        Gold,
        Gem,
        Other,
    }
    private E_GoodsType goodsType = E_GoodsType.All;
    private List<ShopGoods> GoodsList = new List<ShopGoods>();
    void Start()
    {
        GetUiContro<Button>("Button_Back").onClick.AddListener(() =>
        {
            UiManager.GetInstance().HidePanel("ShopPanel");           
        });
        GetUiContro<Button>("Button_Home").onClick.AddListener(() =>
        {
            UiManager.GetInstance().HidePanel("ShopPanel");          
        });
        

        GetUiContro<Text>("GoldText").text = TestPlayer.GetInstance().gold.ToString();
        GetUiContro<Text>("GemText").text = TestPlayer.GetInstance().gem.ToString();
        AddTogglesListener();
        onValueChanged(true);
    }

    protected void onValueChanged(bool b)
    {
        ChangeSeleced();
        DestroyItemList();
        if (GetUiContro<Toggle>("Button_All").isOn)
        {
            ChangeType(E_GoodsType.All);
        }
        else if (GetUiContro<Toggle>("Button_Gold").isOn)
        {
            ChangeType(E_GoodsType.Gold);
        }
        else if (GetUiContro<Toggle>("Button_Gem").isOn)
        {
            ChangeType(E_GoodsType.Gem);
        }
        CreateItem();
    }
    private void ChangeSeleced()
    {
        GetUiContro<Image>("Button_All").sprite = GetUiContro<Toggle>("Button_All").isOn ?ResourcesManager.GetInstance().Load<Sprite>("Ui/Sprite/InventoryPanel/Btn_MenuButton_Rectangle00_s") : ResourcesManager.GetInstance().Load<Sprite>("Ui/Sprite/InventoryPanel/Btn_MenuButton_Rectangle00_n");
        GetUiContro<Image>("Button_Gold").sprite = GetUiContro<Toggle>("Button_Gold").isOn ?ResourcesManager.GetInstance().Load<Sprite>("Ui/Sprite/InventoryPanel/Btn_MenuButton_Rectangle00_s") : ResourcesManager.GetInstance().Load<Sprite>("Ui/Sprite/InventoryPanel/Btn_MenuButton_Rectangle00_n");
        GetUiContro<Image>("Button_Gem").sprite = GetUiContro<Toggle>("Button_Gem").isOn ?ResourcesManager.GetInstance().Load<Sprite>("Ui/Sprite/InventoryPanel/Btn_MenuButton_Rectangle00_s") : ResourcesManager.GetInstance().Load<Sprite>("Ui/Sprite/InventoryPanel/Btn_MenuButton_Rectangle00_n");
    }
    public void ChangeType(E_GoodsType type)
    {
        goodsType = type;
    }
    private void DestroyItemList()
    {
        for (int i = 0; i < GoodsList.Count; i++)
        {
            Destroy(GoodsList[i].gameObject);
        }
        GoodsList.Clear();
    }
    private void CreateItem()
    {
        ShopGoods goods;
        foreach (var dic in ExcelReaderManager.GetInstance().GetTable<Item>().dataDic)
        {                       
            int type= ExcelReaderManager.GetInstance().GetTable<Item>().dataDic[dic.Key].buyType;
            if (goodsType == E_GoodsType.All || (int)goodsType == type)
            {
                goods = ResourcesManager.GetInstance().Load<GameObject>("Ui/Prefabs/ShopGoods").GetComponent<ShopGoods>();
                goods.transform.SetParent(GetUiContro<ScrollRect>("ScrollRect").content);
                goods.Init(dic.Key);
                GoodsList.Add(goods);
            }
        }
    }
    private void AddTogglesListener()
    {
        GetUiContro<Toggle>("Button_All").onValueChanged.AddListener(onValueChanged);
        GetUiContro<Toggle>("Button_Gold").onValueChanged.AddListener(onValueChanged);
        GetUiContro<Toggle>("Button_Gem").onValueChanged.AddListener(onValueChanged);
    }
    public void SetMoney()
    {
        GetUiContro<Text>("GoldText").text = TestPlayer.GetInstance().gold.ToString();
        GetUiContro<Text>("GemText").text = TestPlayer.GetInstance().gem.ToString();
    }
}
