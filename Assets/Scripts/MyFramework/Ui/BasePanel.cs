using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BasePanel : MonoBehaviour
{
    protected Dictionary<string, List<UIBehaviour>> controDic= new Dictionary<string, List<UIBehaviour>>();//key：gameobject名字   value：装ui控件的List
    protected virtual void Awake()
    {
        FindChildsUiComponents<Button>();
        FindChildsUiComponents<Image>();
        FindChildsUiComponents<Toggle>();
        FindChildsUiComponents<Slider>();
        FindChildsUiComponents<ScrollRect>();
        FindChildsUiComponents<Text>();
        FindChildsUiComponents<GridLayoutGroup>();
    }   
    private void FindChildsUiComponents<T>() where T :UIBehaviour
    {
        T[] s = this.GetComponentsInChildren<T>();
        
        for (int i = 0; i < s.Length; i++)
        {
            string name = s[i].gameObject.name;
            if (controDic.ContainsKey(name))
            {
                controDic[name].Add(s[i]);
            }
            else
            {
                controDic.Add(name, new List<UIBehaviour>() { s[i] });
            }
            if(s[i] is Button)
            {
                (s[i] as Button).onClick.AddListener(() =>
                {
                    OnClick(name);
                });
            }else if(s[i] is Toggle)
            {
                (s[i] as Toggle).onValueChanged.AddListener((b) =>
                {
                    onValueChanged(name, b);
                });
            }           
        } 
    }
    /// <summary>
    /// 获取name面板下，T类型的Ui控件
    /// </summary>
    /// <typeparam name="T">Ui控件类型</typeparam>
    /// <param name="name">Ui面板名称</param>
    /// <returns></returns>
    public T GetUiContro<T>(string name) where T : UIBehaviour
    {
        if(controDic.ContainsKey(name))
        {
            for (int i = 0; i < controDic[name].Count; i++)
            {
                if (controDic[name][i] is T)
                {
                    return controDic[name][i] as T;
                }
            }
        }
        return null;
    }
    public virtual void ShowSelf()
    {

    }
    public virtual void HideSelf()
    {

    }
    protected virtual void OnClick(string btnName)
    {

    }
    protected virtual void onValueChanged(string toggleName,bool b)
    {

    }
}
