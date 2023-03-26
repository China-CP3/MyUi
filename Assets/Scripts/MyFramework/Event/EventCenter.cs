using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IMyEvent
{

}
public class EventInfo<T>:IMyEvent
{
    public UnityAction<T> action;
    public EventInfo(UnityAction<T> action)
    {
        this.action = action;
    }
}
public class EventInfo : IMyEvent
{
    public UnityAction action;
    public EventInfo(UnityAction action)
    {
        this.action = action;
    }
}
public class EventCenter:BaseManager<EventCenter>
{
    private static Dictionary<string, IMyEvent> eventDic=new Dictionary<string, IMyEvent>();
    
    public void AddEventListener<T>(string name, UnityAction<T> action)
    {
        if(eventDic.ContainsKey(name))
        { 
            (eventDic[name] as EventInfo<T>).action += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo<T>(action));
        }
    }
    public void AddEventListener(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).action += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo(action));
        }
    }
    public void RemoveEventListener<T>(string name, UnityAction<T> unityAction)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).action -= unityAction;
        }
    }
    public void RemoveEventListener(string name, UnityAction unityAction)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).action -= unityAction;
        }
    }
    public void EventTrigger<T>(string name,T info)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).action?.Invoke(info);
        }
    }
    public void EventTrigger(string name)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).action?.Invoke();
        }
    }
    public void Clear()
    {
        eventDic.Clear();
    }
}
