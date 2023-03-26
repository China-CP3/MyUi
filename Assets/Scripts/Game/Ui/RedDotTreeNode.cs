using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RedDotTreeNode 
{
    public RedDotDePath path;
    public RedDotDePath parentPath;
    public E_RedDotType type;
    public bool redDotActive;
    public int redDotCount;
    public UnityAction<RedDotTreeNode> logicHander;
    public UnityAction<E_RedDotType, bool, int> OnRedDotActiveChange;
    /// <summary>
    /// 根据红点类型进行赋值和显隐
    /// </summary>
    /// <returns></returns>
    public virtual bool RefreshRedDotState()
    {
        if(type== E_RedDotType.NumChildren)
        {
            redDotCount=RedDotManager.GetInstance().GetChildCount(path);
            redDotActive = redDotCount > 0;
        }
        else
        {
            redDotCount = RefreshRedDotCont();
        }
        if(type == E_RedDotType.NumData)
        {
            if(redDotCount>0)
            {
                redDotActive = true;
            }
            else
            {
                redDotActive = false;
            }
        }

        logicHander?.Invoke(this);
        OnRedDotActiveChange?.Invoke(type, redDotActive, redDotCount);
        return redDotActive;
    }
    public virtual int RefreshRedDotCont()
    {
        return 1;
    }
}
