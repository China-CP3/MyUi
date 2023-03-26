using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : BasePanel
{

    // Start is called before the first frame update
    void Start()
    {
        GetUiContro<Button>("Button_Inventory").onClick.AddListener(() =>
        {
            UiManager.GetInstance().ShowPanel<InventoryPanel>("InventoryPanel");
        });
        GetUiContro<Button>("Button_Shop").onClick.AddListener(() =>
        {
            UiManager.GetInstance().ShowPanel<ShopPanel>("ShopPanel");
        });
        GetUiContro<Button>("Button_AddGold").onClick.AddListener(() =>
        {
            TestPlayer.GetInstance().gold += 100;
            GetUiContro<Text>("GoldText").text = TestPlayer.GetInstance().gold.ToString();
        });
        GetUiContro<Button>("Button_AddGem").onClick.AddListener(() =>
        {
            TestPlayer.GetInstance().gem += 100;
            GetUiContro<Text>("GemText").text = TestPlayer.GetInstance().gem.ToString();
        });
        GetUiContro<Text>("LevelText").text = TestPlayer.GetInstance().level.ToString();
        GetUiContro<Text>("NameText").text = TestPlayer.GetInstance().playerName.ToString();
        GetUiContro<Text>("GoldText").text = TestPlayer.GetInstance().gold.ToString();
        GetUiContro<Text>("GemText").text = TestPlayer.GetInstance().gem.ToString();

        CreateItem();
        RedDotTreeNode bagNode = new RedDotTreeNode { type = E_RedDotType.NumData, path = RedDotDePath.bag, logicHander = OnRedDotBagLogicHandler };
        RedDotManager.GetInstance().Init(new List<RedDotTreeNode> { bagNode });
        
    }
    private void CreateItem()
    {
        int totalNum;//����Ʒ������
        int yu;
        int length;
        foreach (var dic in TestPlayer.GetInstance().dicItem)
        {
             totalNum = dic.Value;//����Ʒ������               
             if (totalNum <= 99)
             {
                 ResItem(dic.Key, dic.Value);
             }
             else
             {
                 yu = totalNum % 99;
                 length = (totalNum - yu) / 99;//��Ʒ��Ҫ�ֳ�length+1�� ���1�ѵĸ�������yu
                 for (int i = 0; i < length; i++)
                 {
                     ResItem(dic.Key, 99);
                 }
                 ResItem(dic.Key, yu);
             }
        }
    }
    private void ResItem(int id, int num)
    {
        ItemId item = new ItemId(id, num);
        RedDotDataTest.itemList.Add(item);
    }
    public void OnRedDotBagLogicHandler(RedDotTreeNode redNode)
    {
        redNode.redDotCount = 0;
        foreach (var item in RedDotDataTest.itemList)
        {
            if (item.isRead == false)
            {
                redNode.redDotCount++;
            }
        }
    }
    public void SetMoney()
    {
        GetUiContro<Text>("GoldText").text = TestPlayer.GetInstance().gold.ToString();
        GetUiContro<Text>("GemText").text = TestPlayer.GetInstance().gem.ToString();
    }
}
