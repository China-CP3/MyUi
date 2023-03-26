using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ScenesManager : BaseManager<ScenesManager>
{
    public void LoadScene(string sceneName,UnityAction action)
    {
        SceneManager.LoadScene(sceneName);
        action();
    }
    public void LoadSceneAsync(string sceneName, UnityAction action = null)
    {
        MonoManager.GetInstance().StartCoroutine(ReallyLoadSceneAsync(sceneName, action));
    }
    private IEnumerator ReallyLoadSceneAsync(string sceneName, UnityAction action=null)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        while(!ao.isDone)
        {
            EventCenter.GetInstance().EventTrigger("进度条更新",ao.progress);
            yield return 1;
        }
        action?.Invoke();
    }

}
