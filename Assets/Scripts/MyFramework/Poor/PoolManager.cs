using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PoolData
{
    public GameObject fatherObj;
    public List<GameObject> poolList;
    public PoolData(GameObject obj,GameObject poolRoot)
    {
        fatherObj = new GameObject(obj.name);
        fatherObj.transform.SetParent(poolRoot.transform);
        poolList = new List<GameObject>();
        PushObj(obj);
    }
    public void PushObj(GameObject obj)
    {
        obj.SetActive(false);
        poolList.Add(obj);
        obj.transform.SetParent(fatherObj.transform);
    }
    public GameObject GetObj()
    {
        GameObject obj = null;
        obj=poolList[poolList.Count-1];
        obj.SetActive(true);
        poolList.RemoveAt(poolList.Count - 1);
        obj.transform.SetParent(null);
        return obj;
    }
}
public class PoolManager:BaseManager<PoolManager>
{
    private Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();
    private GameObject PoolRoot;
    
    public GameObject GetGameObject(string name)
    {
        if (poolDic.ContainsKey(name) && poolDic[name].poolList.Count>0)
        {
            return poolDic[name].GetObj();
        }
        else
        {
            return ResourcesManager.GetInstance().Load<GameObject>(name);
        }
    }

    public void PushToPoor(string name,GameObject obj)
    {
        if(PoolRoot==null)
        {
            PoolRoot = new GameObject("PoolRoot");
        }

        if (poolDic.ContainsKey(name))
        {
            poolDic[name].PushObj(obj);
        }
        else
        {
            poolDic.Add(name, new PoolData(obj, PoolRoot));
        }
    }

    public void Clear()
    {
        poolDic.Clear();
        PoolRoot = null;
    }
}
