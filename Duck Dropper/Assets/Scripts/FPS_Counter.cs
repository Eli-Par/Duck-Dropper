using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPS_Counter : MonoBehaviour
{
    [SerializeField] private float refreshDelay = default;
    [SerializeField] private TextMeshProUGUI fpsText = default;
    [SerializeField] private string numberFormat = "00.00";

    private float refreshTimer = 0;
    private bool showFPS = false;

    private float totalFPS = 0;
    private int framesCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        fpsText.gameObject.SetActive(showFPS);
        refreshTimer = refreshDelay;
    }

    // Update is called once per frame
    void Update()
    {
        //Toggle the fps counter when F is pressed
        if(Input.GetKeyDown(KeyCode.F))
        {
            showFPS = !showFPS;
            fpsText.gameObject.SetActive(showFPS);
        }

        //Only do the fps counter logic if the counter is shown.
        //If it isn't set the timer to the delay value so it refreshes on the first frame it's activated. The variables used for the average are also reset.
        if(showFPS)
        {
            //Calculate the last frames fps value using unscaled time
            float fps = 1f / Time.unscaledDeltaTime;

            //Add the frames value to the total of fps values and the count of values so an average can be calculated
            totalFPS += fps;
            framesCount++;

            //Increase the timer by the time that has passed
            refreshTimer += Time.unscaledDeltaTime;

            //Update the fps counter if the timer is greater than the refresh delay
            if(refreshTimer >= refreshDelay)
            {
                //Calculate the average fps since the last refresh
                float average = totalFPS / framesCount;

                //Update the UI text
                fpsText.text = average.ToString(numberFormat) + " FPS";

                //Reset the timer to 0
                refreshTimer = 0;

                //Refresh the variables used to track the average
                totalFPS = 0;
                framesCount = 0;
            }
            
        }
        else
        {
            refreshTimer = refreshDelay;

            totalFPS = 0;
            framesCount = 0;
        }

    }
}
