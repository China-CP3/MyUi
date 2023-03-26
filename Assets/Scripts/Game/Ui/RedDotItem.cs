using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedDotItem : BasePanel
{
    public RedDotDePath path;
    public GameObject redDotObj;
    public Text countText;
    void Start()
    {
        RedDotManager.GetInstance().AddListenerRedDotChange(path, OnRedDotStateChangeEvent);
        RedDotManager.GetInstance().UpdateRedDotState(path);
    }

    public void OnRedDotStateChangeEvent(E_RedDotType type, bool active, int count)
    {
        redDotObj.SetActive(active);
        if (type == E_RedDotType.NumChildren|| type == E_RedDotType.NumData)
        {
            countText.text = count.ToString();
        }
        countText.gameObject.SetActive(type != E_RedDotType.Normal);
    }
    public void OnEnable()
    {
        RedDotManager.GetInstance().UpdateRedDotState(path);
    }
    public void OnDestroy()
    {
        RedDotManager.GetInstance().RemoveListenerRedDotChange(path, OnRedDotStateChangeEvent);
    }
}
