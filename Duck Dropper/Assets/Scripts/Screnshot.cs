using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Screnshot : MonoBehaviour
{
    [SerializeField] private int superSize = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F2))
        {
            string name = "Duck Dropper " + DateTime.Now.ToString("yyyy/MM/dd HH-mm-ss") + ".png";
            ScreenCapture.CaptureScreenshot(name, superSize);
            Debug.Log("Screenshot " + name + " taken");
        }
    }
}
