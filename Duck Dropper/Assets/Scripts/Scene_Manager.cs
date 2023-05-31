using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Manager : MonoBehaviour
{
    public static Scene_Manager Instance { get; private set; }

    [SerializeField] private GameObject transitionObj = default;
    [SerializeField] private Animator transitionAnimator = default;
    [SerializeField] private float transitionStartTime = 1f;
    [SerializeField] private float transitionEndTime = 1f;

    [SerializeField] private string startAnimName = default;
    [SerializeField] private string endAnimName = default;

    private void Awake()
    {
        //Singleton pattern so there is only one gameobject with this code
        DontDestroyOnLoad(this);
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        //Hide to begin with
        transitionObj.SetActive(false);
    }

    public void ChangeScene(int sceneNum)
    {
        StartCoroutine(TransitionChangeScene(sceneNum));
    }

    public void ReloadScene()
    {
        ChangeScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Changes scenes to the specified index while doing a transition animation
    IEnumerator TransitionChangeScene(int index)
    {
        //Enable the transition object and play the first half of the animation
        transitionObj.SetActive(true);
        transitionAnimator.SetTrigger(startAnimName);

        //Wait the length of the animation
        yield return new WaitForSecondsRealtime(transitionStartTime);

        //Load the scene
        SceneManager.LoadScene(index);

        //Wait until the scene is done loading
        while (SceneManager.GetActiveScene().buildIndex != index)
        {
            yield return null;
        }

        //Play the second half of the transition animation
        transitionAnimator.SetTrigger(endAnimName);

        //Wait for the animation to be done and disable the transition object
        yield return new WaitForSecondsRealtime(transitionEndTime);
        transitionObj.SetActive(false);
    }
}
