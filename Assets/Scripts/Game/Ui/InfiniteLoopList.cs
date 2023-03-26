using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
/// <summary>
/// ����������̳�����ӿ�
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IItemBase<T>
{
    void InitInfo(T item);
}
/// <summary>
/// 
/// </summary>
/// <typeparam name="T">������Դ��</typeparam>
/// <typeparam name="K">�����еĸ�����</typeparam>
public class InfiniteLoopList<T, K> where K : IItemBase<T>
{
    private RectTransform content;
    private int ViewHight;
    public int rowNum;//1�еĸ���
    private int itemWeight;//��Ʒ�Ŀ�
    private int itemHight;//��Ʒ�ĸ�
    private int rowSpacing;//�м��
    private int columnSpacing;//�м��
    private int oldMinIndex = -1;
    private int oldMaxIndex = -1;   
    private Dictionary<int, GameObject> dic=new Dictionary<int, GameObject>();//�洢������ʾ����Ʒ
    public List<T> itemList;//����Ӧ�ñ���ʾ����Ʒ
    private string itemPath;//Ԥ�Ƽ���·��  ���米���еĸ���
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
        
        //content��y������Ʒ�߶ȿ��Եõ� ��������Ұ��Χ���к�ĿǰӦ����ʾ�ڼ��� ��0��ʼ��
        //����200/200=1 ˵��������1�� Ӧ����ʾlist�е�1�� Ҳ������ʵ�ﾳ��2�� 
        //�õ������� �ٳ���ÿ����ʾ�ĸ��� �Ϳ��Եõ����Ͻǵ�һ����Ʒ��index
        //���� 1*3=3 ˵�����Ͻǵ�1����Ʒindex��3 ��0��ʼ�� Ҳ������ʵ�ﾳ��4�����ڶ��еĵ�1��
        int minIndex = (int)(content.anchoredPosition.y / (itemHight + rowSpacing)) * rowNum;//һ��Ҫ������Ʒ�߶�+�м�� ������BUG 
        int maxIndex = (int)((content.anchoredPosition.y + ViewHight) / itemHight) * rowNum + rowNum - 1;//�õ����һ�е����һ����Ʒindex

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
