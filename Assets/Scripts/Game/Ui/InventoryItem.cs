using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public enum E_EquipType
{
    all,
    weapon,
    shield,
    helmet,
    dress,
}
public class InventoryItem : BasePanel,IItemBase<ItemId>
{
    public int id;
    public int num;
    public ItemId item;
    public GameObject redDotObj;
    private void Start()
    {

    }
    public void InitInfo(ItemId item)
    {
        this.id = item.id;
        this.num = item.num;
        this.item = item;
        redDotObj.SetActive(item.isRead == false);
        GetUiContro<Image>("ItemIcon").sprite = ResourcesManager.GetInstance().Load<Sprite>(ExcelReaderManager.GetInstance().GetTable<Item>().dataDic[item.id].icon);
        GetUiContro<Text>("Text_Value").text = item.num.ToString();

    }

    protected override void OnClick(string btnName)
    {
        UiManager.GetInstance().GetPanel<InventoryPanel>("InventoryPanel").SetTips(id, num);
        if (this.item.isRead == false)
        {
            this.item.isRead = true;
            redDotObj.SetActive(false);
            RedDotManager.GetInstance().UpdateRedDotState(RedDotDePath.bag);
        }
    }

}
