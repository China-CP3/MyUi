using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum E_RedDotType
{
    Normal,
    NumChildren,//根据子节点数量显示数字
    NumData,//根据赋值显示数字
}
public class RedDotManager : BaseManager<RedDotManager>
{
    private Dictionary<RedDotDePath, RedDotTreeNode> allNodesDic= new Dictionary<RedDotDePath, RedDotTreeNode>();   
    public void Init(List<RedDotTreeNode> listNode)
    {
        foreach (var node in listNode)
        {
            allNodesDic.Add(node.path,node);
        }
    }
    public void AddListenerRedDotChange(RedDotDePath path,UnityAction<E_RedDotType,bool,int> callBack)
    {
        if(allNodesDic.TryGetValue(path,out RedDotTreeNode node))
        {
            node.OnRedDotActiveChange+=callBack;
        }
        else
        {
            Debug.LogError($"key:{path} not exits!");
        }
    }
    public void RemoveListenerRedDotChange(RedDotDePath path, UnityAction<E_RedDotType, bool, int> callBack)
    {
        if (allNodesDic.TryGetValue(path, out RedDotTreeNode node))
        {
            node.OnRedDotActiveChange -= callBack;
        }
        //else
        //{
        //    Debug.LogError($"key:{path} not exits!");
        //}
    }
    /// <summary>
    /// 递归更新红点以及父节点的状态
    /// </summary>
    /// <param name="path"></param>
    public void UpdateRedDotState(RedDotDePath path)
    {
        if (path == RedDotDePath.none)
        {
            return;
        }
        if (allNodesDic.TryGetValue(path, out RedDotTreeNode node))
        {
            node.RefreshRedDotState();
            UpdateRedDotState(node.parentPath);
        }
    }
    /// <summary>
    /// 获得子节点红点个数
    /// </summary>
    /// <param name="path"></param>
    public int GetChildCount(RedDotDePath path)
    {
        int count = 0;
        ComputeChildCount(path,ref count);
        return count;
    }
    /// <summary>
    /// 计算子节点红点个数
    /// </summary>
    /// <param name="path"></param>
    private void ComputeChildCount(RedDotDePath path,ref int count)
    {
        foreach (var item in allNodesDic.Values)
        {
            if(item.parentPath==path)
            {
                item.RefreshRedDotState();
                if(item.redDotActive)
                {
                    count += item.redDotCount;
                    if(item.type!=E_RedDotType.NumData)
                    {
                        ComputeChildCount(item.path,ref count);
                    }
                }
            }
        }
    }
}
