using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Screnshot : MonoBehaviour
{
    [SerializeField] private int superSize = 1;
    [SerializeField] private TextMeshProUGUI text = default;
    [SerializeField] private float showTime = 1f;

    private string lastName = "";
    private float timer = 100;

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
            lastName = name;
            ScreenCapture.CaptureScreenshot(name, superSize);
            Debug.Log("Screenshot " + name + " taken");
            timer = 0;
        }

        if(timer < showTime)
        {
            text.text = lastName;
            text.gameObject.SetActive(true);
        }
        else
        {
            text.gameObject.SetActive(false);
        }

        timer += Time.unscaledDeltaTime;
    }
}
