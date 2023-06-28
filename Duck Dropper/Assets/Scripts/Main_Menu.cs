using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Menu : MonoBehaviour
{
    public int gameSceneIndex = 1;

    [Space]

    public RectTransform canvasTransform;
    public RectTransform instructionTransform;

    [Space]
    public RectTransform menusTransform;
    public float screenOffset = 600;
    public int currentOffsetMultiplier = 0;
    public float screenTransitionSpeed = 600;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Make the width of the instruction box the same as the canvas width
        float width = canvasTransform.rect.width;
        instructionTransform.sizeDelta = new Vector2(width, instructionTransform.sizeDelta.y);

        //Get the position of the menus
        float yPos = menusTransform.anchoredPosition.y;

        //If the menu is close to the target position, set the position to the target position
        if(Mathf.Abs(screenOffset * currentOffsetMultiplier - yPos) < screenTransitionSpeed * Time.deltaTime)
        {
            yPos = screenOffset * currentOffsetMultiplier;
        }
        else
        {
            //If the position is under the position, move it up, otherwise move it down
            if(yPos < screenOffset * currentOffsetMultiplier)
            {
                yPos += screenTransitionSpeed * Time.deltaTime;
            }
            else
            {
                yPos -= screenTransitionSpeed * Time.deltaTime;
            }
        }

        //Set the position of the menus to apply the changed position
        menusTransform.anchoredPosition = new Vector2(menusTransform.anchoredPosition.x, yPos);

    }

    public void StartGame()
    {
        Scene_Manager.Instance.ChangeScene(gameSceneIndex);
    }

    public void Quit()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }

    public void SetScreenMultiplier(int mult)
    {
        currentOffsetMultiplier = mult;
    }
}
