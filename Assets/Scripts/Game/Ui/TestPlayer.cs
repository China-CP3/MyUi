using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayer:SingletonAutoMono<TestPlayer>
{
    public Dictionary<int,int> dicItem=new Dictionary<int,int>(); //背包物品的ID和数量
    public List<InventoryItem> items;//玩家已穿戴的装备
    public string playerName;
    public int level;  
    public int gold;
    public int gem;
    public int atk;
    public int def;
    public int hp;
    private void Awake()
    {
        items = new List<InventoryItem>();
        playerName = ExcelReaderManager.GetInstance().GetTable<Player>().dataDic[1].name;
        level = ExcelReaderManager.GetInstance().GetTable<Player>().dataDic[1].level;
        hp = ExcelReaderManager.GetInstance().GetTable<Player>().dataDic[1].hp;
        def = ExcelReaderManager.GetInstance().GetTable<Player>().dataDic[1].def;
        atk= ExcelReaderManager.GetInstance().GetTable<Player>().dataDic[1].atk;
        gold = ExcelReaderManager.GetInstance().GetTable<Player>().dataDic[1].gold;
        gem = ExcelReaderManager.GetInstance().GetTable<Player>().dataDic[1].gem;
        string[] strs = ExcelReaderManager.GetInstance().GetTable<Player>().dataDic[1].item.Split(',');
        string[] s;
        for (int i = 0; i < strs.Length; i++)
        {
            s = strs[i].Split(':');
            dicItem.Add(int.Parse(s[0]), int.Parse(s[1]));
        }
    }
    public void AddEquip(InventoryItem item)
    {
        items.Add(item);
        hp += ExcelReaderManager.GetInstance().GetTable<Item>().dataDic[item.id].hp;
        atk += ExcelReaderManager.GetInstance().GetTable<Item>().dataDic[item.id].atk;
        def += ExcelReaderManager.GetInstance().GetTable<Item>().dataDic[item.id].def;
    }
    public void RemoveEquip(InventoryItem item)
    {
        items.Remove(item);
        hp -= ExcelReaderManager.GetInstance().GetTable<Item>().dataDic[item.id].hp;
        atk -= ExcelReaderManager.GetInstance().GetTable<Item>().dataDic[item.id].atk;
        def -= ExcelReaderManager.GetInstance().GetTable<Item>().dataDic[item.id].def;
    }
}
