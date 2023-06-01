using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause_Menu : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private GameObject backgroundPanel = default;
    [SerializeField] private Animator backgroundAnimator = default;
    [SerializeField] private float fadeInAbortTime = default;
    [SerializeField] private float fadeOutTime = default;

    [SerializeField] private GameObject buttons = default;

    [Space]

    [Header("Menu Properties")]
    public static bool isPaused = false;

    private float timeSincePaused = 0;

    [SerializeField] private int menuSceneIndex = default;

    // Start is called before the first frame update
    void Start()
    {
        //Set elements of pause menu to match if the menu is active
        backgroundPanel.SetActive(isPaused);
        buttons.SetActive(isPaused);
    }

    // Update is called once per frame
    void Update()
    {
        //When the escape key is pressed, toggle the pause menu
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        //If paused, increase the time the pause menu has been open, otherwise set the time back to 0
        if(isPaused)
        {
            timeSincePaused += Time.unscaledDeltaTime;
        }
        else
        {
            timeSincePaused = 0;
        }
    }

    //Remove the pause menu and continue the game
    public void Resume()
    {
        //Change the pause bool, reset the time scale and hide all buttons
        isPaused = false;
        Time.timeScale = 1f;
        buttons.SetActive(false);

        //If the fade in animation is past the time to abort, play the fade out animation and after a delay disable the background
        //Otherwise immediately remove the background panel
        if(fadeInAbortTime <= timeSincePaused)
        {
            backgroundAnimator.SetTrigger("FadeOut");
            StartCoroutine(DisableBackground());
        }
        else
        {
            backgroundPanel.SetActive(false);
        }
        
    }

    //Wait the fade out time and disable the background panel
    IEnumerator DisableBackground()
    {
        yield return new WaitForSecondsRealtime(fadeOutTime);

        backgroundPanel.SetActive(false);
    }

    void Pause()
    {
        //Change the pause bool, reset the time scale and show all buttons
        isPaused = true;
        Time.timeScale = 0f;
        buttons.SetActive(true);

        //Show the background panel (Animates automatically)
        backgroundPanel.SetActive(true);
    }

    public void Restart()
    {
        //Turn off the pause menu and reset the time scale
        isPaused = false;
        Time.timeScale = 1f;

        //Reload the active scene
        Scene_Manager.Instance.ReloadScene();
    }

    public void Menu()
    {
        //Turn off the pause menu and reset the time scale
        isPaused = false;
        Time.timeScale = 1f;

        //Load the menu scene
        Scene_Manager.Instance.ChangeScene(menuSceneIndex);
    }

    public void Quit()
    {
        Debug.Log("Quitting");
        Application.Quit();
    }
}
