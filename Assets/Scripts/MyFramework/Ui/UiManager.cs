using System.Collections;
using System.Collections.Generic;
using UnityEditor.EventSystems;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public enum E_Ui_Layer
{
    Bottom,
    Middle,
    Top,
    Tips,

}

public class UiManager : BaseManager<UiManager>
{
    private Dictionary<string,BasePanel> dicPanel=new Dictionary<string,BasePanel>();
    
    private Transform Bottom;
    private Transform Middle;
    private Transform Top;
    private Transform Tips;

    private RectTransform canvas;
    private GameObject eventSystem;

    public UiManager()
    {
        //加载出来Canvas和EventSystem 过场景时保留
        canvas = ResourcesManager.GetInstance().Load<GameObject>("Ui/Prefabs/Canvas").transform as RectTransform;
        eventSystem =ResourcesManager.GetInstance().Load<GameObject>("Ui/Prefabs/EventSystem");
        Object.DontDestroyOnLoad(canvas);
        Object.DontDestroyOnLoad(eventSystem);

        //找到各个UI层级
        Bottom = canvas.transform.Find("Bottom");
        Middle = canvas.transform.Find("Middle");
        Top = canvas.transform.Find("Top");
        Tips = canvas.transform.Find("Tips");

    }
    public void ShowPanel<T>(string panelName,E_Ui_Layer ui_Layer = E_Ui_Layer.Bottom, UnityAction<T> callBack = null) where T : BasePanel
    {
        if (dicPanel.ContainsKey(panelName))
        {
            dicPanel[panelName].ShowSelf();
            callBack?.Invoke(dicPanel[panelName] as T);
            return;
        }
        ResourcesManager.GetInstance().LoadAsync<GameObject>("Ui/Prefabs/" + panelName, (panel) =>
        {
            switch (ui_Layer)
            {
                case E_Ui_Layer.Bottom:
                    panel.transform.SetParent(Bottom);
                    break;
                case E_Ui_Layer.Middle:
                    panel.transform.SetParent(Middle);
                    break;
                case E_Ui_Layer.Top:
                    panel.transform.SetParent(Top);
                    break;
                case E_Ui_Layer.Tips:
                    panel.transform.SetParent(Tips);
                    break;
            }
            panel.transform.localPosition = Vector3.zero;
            panel.transform.localScale = Vector3.one;
            (panel.transform as RectTransform).offsetMin = Vector2.zero;
            (panel.transform as RectTransform).offsetMax = Vector2.zero;

            T panelScript = panel.GetComponent<T>();
            if (callBack != null)
            {
                callBack(panelScript);
            }
            panelScript.ShowSelf();
            dicPanel.Add(panelName, panelScript);          
        });
    }
    public void HidePanel(string name)
    {
        if (dicPanel.ContainsKey(name))
        {
            dicPanel[name].HideSelf();
            Object.Destroy(dicPanel[name].gameObject);
            dicPanel.Remove(name);            
        }
    }
    public T GetPanel<T>(string name) where T:BasePanel
    {
        if(dicPanel.ContainsKey(name))
        {
            return dicPanel[name] as T;
        }
        return null;
    }
    public Transform GetLayer(E_Ui_Layer ui_Layer)
    {
        switch (ui_Layer)
        {
            case E_Ui_Layer.Bottom:
                return Bottom;
            case E_Ui_Layer.Middle:
                return Middle;
            case E_Ui_Layer.Top:
                return Top;
            case E_Ui_Layer.Tips:
                return Tips;
            default:
                return null;
        }
    }
    public void AddEventTriggerListener(UIBehaviour ui,EventTriggerType type,UnityAction<BaseEventData> callBack)
    {
        EventTrigger eventTrigger;
        if (!(eventTrigger = ui.gameObject.GetComponent<EventTrigger>()))
        {
           eventTrigger = ui.gameObject.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callBack);

        eventTrigger.triggers.Add(entry);
    }

}
