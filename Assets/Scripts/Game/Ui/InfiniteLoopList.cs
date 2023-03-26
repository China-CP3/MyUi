using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
/// <summary>
/// 背包格子类继承这个接口
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IItemBase<T>
{
    void InitInfo(T item);
}
/// <summary>
/// 
/// </summary>
/// <typeparam name="T">数据来源类</typeparam>
/// <typeparam name="K">背包中的格子类</typeparam>
public class InfiniteLoopList<T, K> where K : IItemBase<T>
{
    private RectTransform content;
    private int ViewHight;
    public int rowNum;//1行的个数
    private int itemWeight;//物品的宽
    private int itemHight;//物品的高
    private int rowSpacing;//行间距
    private int columnSpacing;//列间距
    private int oldMinIndex = -1;
    private int oldMaxIndex = -1;   
    private Dictionary<int, GameObject> dic=new Dictionary<int, GameObject>();//存储正在显示的物品
    public List<T> itemList;//所有应该被显示的物品
    private string itemPath;//预制件的路径  比如背包中的格子
    public void Init(List<T> items, string itemPath= "Ui/Prefabs/InventoryItem")
    {
        this.itemPath = itemPath;
        itemList = items;
        int h = (Mathf.CeilToInt(itemList.Count / (float)rowNum) * (itemHight+ rowSpacing));
        content.sizeDelta = new Vector2(0,h);
        content.anchoredPosition = new Vector2(0,0);
        foreach (var item in dic)
        {
            //Destroy(item.Value);
            PoolManager.GetInstance().PushToPoor(itemPath, item.Value);
        }
        dic.Clear();
    }
    public void SetContentAndViewHight(RectTransform content,int hight)
    {
        this.content = content;
        ViewHight = hight;
    }
    public void SetItemSize(int itemWeight=230,int itemHigth=230,int rowSpacing=10,int columnSpacing=40,int rowNum=5)
    {
        this.rowNum = rowNum;
        this.itemWeight = itemWeight;
        this.itemHight = itemHigth;
        this.rowSpacing = rowSpacing;
        this.columnSpacing = columnSpacing;
    }
    public void CheckShowOrHide()
    {
        
        //content的y除以物品高度可以得到 超出了视野范围几行和目前应该显示第几行 从0开始数
        //比如200/200=1 说明超出了1行 应该显示list中第1行 也就是现实语境第2行 
        //得到行数后 再乘以每行显示的个数 就可以得到左上角第一个物品的index
        //比如 1*3=3 说明左上角第1个物品index是3 从0开始数 也就是现实语境第4个，第二行的第1个
        int minIndex = (int)(content.anchoredPosition.y / (itemHight + rowSpacing)) * rowNum;//一定要除以物品高度+行间距 否则有BUG 
        int maxIndex = (int)((content.anchoredPosition.y + ViewHight) / itemHight) * rowNum + rowNum - 1;//得到最后一行的最后一个物品index

        if (minIndex < 0)
        {
            minIndex = 0;
        }

        if (maxIndex >= itemList.Count)
        {
            maxIndex = itemList.Count-1;
        }

        if(minIndex!=oldMinIndex||maxIndex!=oldMaxIndex)
        {
            for (int i = oldMinIndex; i < minIndex; i++)
            {
                if (dic.ContainsKey(i))
                {
                    PoolManager.GetInstance().PushToPoor(itemPath, dic[i]);
                    dic.Remove(i);
                }
            }
            for (int i = maxIndex + 1; i <= oldMaxIndex; i++)
            {
                if (dic.ContainsKey(i))
                {
                    PoolManager.GetInstance().PushToPoor(itemPath, dic[i]);
                    dic.Remove(i);
                }
            }
            oldMinIndex = minIndex;
            oldMaxIndex = maxIndex;
        }
        

        GameObject obj;
        for (int i = minIndex; i <= maxIndex; i++)
        {
            if (dic.ContainsKey(i))
            {
                continue;
            }
            else
            {
                //itemList[i].gameObject.SetActive(true);
                obj = PoolManager.GetInstance().GetGameObject(itemPath);
                obj.GetComponent<K>().InitInfo(itemList[i]);
                obj.transform.SetParent(content);
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = new Vector3((i % rowNum) * (itemWeight + columnSpacing), 
                    (-i / rowNum) * (itemHight+rowSpacing), 0);
                dic.Add(i,obj.gameObject);
            }
        }
    }
}
