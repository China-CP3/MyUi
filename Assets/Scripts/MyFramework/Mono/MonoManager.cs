using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoManager : SingletonAutoMono<MonoManager>
{
    private UnityAction unityAction;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(unityAction!=null)
        {
            unityAction();
        }
    }
    public void AddListener(UnityAction action)
    {
        unityAction+=action;
    }
    public void RemoveListener(UnityAction action)
    {
        unityAction -= action;
    }
}
