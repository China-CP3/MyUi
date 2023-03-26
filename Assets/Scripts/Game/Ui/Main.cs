using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UiManager.GetInstance().ShowPanel<MainPanel>("MainPanel",E_Ui_Layer.Bottom);
    }
}
