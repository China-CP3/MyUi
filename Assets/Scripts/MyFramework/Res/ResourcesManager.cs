using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResourcesManager : BaseManager<ResourcesManager>
{

    public T Load<T>(string name) where T : Object
    {
        T res = Resources.Load<T>(name);
        if(res is GameObject)
        {
           return Object.Instantiate(res);
        }
        return res;
    }
    
    public void LoadAsync<T>(string name,UnityAction<T> callBack) where T : Object
    {       
        MonoManager.GetInstance().StartCoroutine(RellyLoadAsync(name, callBack));              
    }
    private IEnumerator RellyLoadAsync<T>(string name,UnityAction<T> callBack) where T : Object
    {
        ResourceRequest r = Resources.LoadAsync<T>(name);
        yield return r;
        if (r.asset is GameObject)
        {
            callBack(Object.Instantiate(r.asset as T));
        }
        else
        {
            callBack(r.asset as T);
        }
    }
}
