using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;
using Image = UnityEngine.UI.Image;
using Text = UnityEngine.UI.Text;

public enum E_ItemType 
{
    All,
    Item,
    Equip,
    Other,
}
public class ItemId
{
    public int id;
    public int num;
    public bool isRead;
    public ItemId(int i,int n)
    {
        id = i;
        num = n;
        isRead = false;
    }
}
public class InventoryPanel : BasePanel
{
    private E_ItemType itemType = E_ItemType.All;
    public List<ItemId> itemShowingList;//所有应该被显示的物品
    private InfiniteLoopList<ItemId,InventoryItem> loopList;//循环列表
    private void Start()
    {
        GetUiContro<Button>("Button_Close").onClick.AddListener(() =>
        {
            UiManager.GetInstance().HidePanel("InventoryPanel");
        }); 
        AddTogglesListener();

        loopList = new InfiniteLoopList<ItemId, InventoryItem>();
        loopList.SetItemSize();
        loopList.SetContentAndViewHight(GetUiContro<ScrollRect>("ScrollRect").content, 760);
        itemShowingList = new List<ItemId>();     
        onValueChanged(true);
        
    }

    private void Update()
    {
        loopList.CheckShowOrHide();
    }
    protected void onValueChanged(bool b)
    {
        ChangeSeleced();
        DestroyItem();
        if (GetUiContro<Toggle>("btnAll").isOn)
        {
            ChangeType(E_ItemType.All);
        }
        else if(GetUiContro<Toggle>("btnEquip").isOn)
        {
            ChangeType(E_ItemType.Equip);    
        }
        else if (GetUiContro<Toggle>("btnItem").isOn)
        {
            ChangeType(E_ItemType.Item);       
        }
        else if (GetUiContro<Toggle>("btnOther").isOn)
        {
            ChangeType(E_ItemType.Other);       
        }
        CreateItem();
    }
    public void ChangeType(E_ItemType type)
    {
        itemType = type;
    }
    private void DestroyItem()
    {
        itemShowingList.Clear();
    }
    private void CreateItem()
    {       
        int totalNum;//该物品总数量
        int yu;
        int length;
        foreach (var dic in TestPlayer.GetInstance().dicItem)
        {
            if (itemType == E_ItemType.All || ExcelReaderManager.GetInstance().GetTable<Item>().dataDic[dic.Key].type == (int)itemType)
            {
                totalNum = dic.Value;//该物品总数量               
                if(totalNum<=99)
                {
                    ResItem(dic.Key, dic.Value);
                }
                else
                {
                    yu = totalNum % 99;
                    length = (totalNum - yu) / 99;//物品需要分成length+1堆 最后1堆的个数就是yu
                    for (int i = 0; i < length; i++)
                    {
                        ResItem(dic.Key, 99);
                    }
                    ResItem(dic.Key, yu);
                }              
            }
        }

        //loopList.Init(itemShowingList);
        loopList.Init(RedDotDataTest.itemList);
    }

    private void ChangeSeleced()
    {
        GetUiContro<Image>("SelecedBarbtnAll").gameObject.SetActive(GetUiContro<Toggle>("btnAll").isOn);
        GetUiContro<Image>("SelecedBarbtnEquip").gameObject.SetActive(GetUiContro<Toggle>("btnEquip").isOn);
        GetUiContro<Image>("SelecedBarbtnItem").gameObject.SetActive(GetUiContro<Toggle>("btnItem").isOn);
        GetUiContro<Image>("SelecedBarbtnOther").gameObject.SetActive(GetUiContro<Toggle>("btnOther").isOn);
    }
    private void AddTogglesListener()
    {
        GetUiContro<Toggle>("btnAll").onValueChanged.AddListener(onValueChanged);
        GetUiContro<Toggle>("btnEquip").onValueChanged.AddListener(onValueChanged);
        GetUiContro<Toggle>("btnItem").onValueChanged.AddListener(onValueChanged);
        GetUiContro<Toggle>("btnOther").onValueChanged.AddListener(onValueChanged);
    }
    public void SetTips(int id, int num)
    {
        GetUiContro<Image>("Icon").sprite = ResourcesManager.GetInstance().Load<Sprite>(ExcelReaderManager.GetInstance().GetTable<Item>().dataDic[id].icon);
        GetUiContro<Text>("num").text = "数量:"+num.ToString();
        GetUiContro<Text>("name").text = ExcelReaderManager.GetInstance().GetTable<Item>().dataDic[id].name;
        GetUiContro<Text>("tipsText").text = ExcelReaderManager.GetInstance().GetTable<Item>().dataDic[id].tips;
    }
    private void ResItem(int id,int num)
    {
        ItemId item=new ItemId(id,num);
        itemShowingList.Add(item);
    }
}
