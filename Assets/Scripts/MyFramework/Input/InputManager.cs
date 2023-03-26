using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : BaseManager<InputManager>
{
    private bool isStart = true;
    public InputManager()
    {
        MonoManager.GetInstance().AddListener(MyUpdate);
    }
    public void StartOrEnd(bool isStart)
    {
        this.isStart = isStart;
    }
    private void CheckKeyCode(KeyCode key)
    {
        if(Input.GetKeyDown(key))
            EventCenter.GetInstance().EventTrigger<KeyCode>("°´ÏÂ" + key.ToString(), key);
        if (Input.GetKeyUp(key))
            EventCenter.GetInstance().EventTrigger<KeyCode>("Ì§Æð" + key.ToString(), key);
    }
    private void MyUpdate()
    {
        if(isStart==false)
        {
            return;
        }
        CheckKeyCode(KeyCode.W);
        CheckKeyCode(KeyCode.S);
        CheckKeyCode(KeyCode.A);
        CheckKeyCode(KeyCode.D);

    }
}
