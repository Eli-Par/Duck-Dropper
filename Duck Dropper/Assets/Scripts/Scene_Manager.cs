using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_Manager : MonoBehaviour
{
    public static Scene_Manager Instance { get; private set; }

    [Header("Transition Durations")]
    [SerializeField] private float transitionStartTime = 1f;
    [SerializeField] private float transitionEndTime = 1f;

    [Space]
    [Header("RectTransforms")]
    public RectTransform canvasTransform;
    public RectTransform transitionTransform;
    public RectTransform duckTransform;

    [Space]
    [Header("Transform Animation")]
    [Tooltip("The offset of the duck image from the rectangle image")]
    public Vector2 offset;

    public Vector2 startPos;
    public Vector2 endPos;

    public Vector2 aspectRatio;
    public Vector2 aspectRatioDuck;
    public float scaleMult = 1;

    public int easePow = 3;

    [HideInInspector] public static bool transitionActive = false;

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

        transitionTransform.gameObject.SetActive(true);
        duckTransform.gameObject.SetActive(true);

        transitionTransform.sizeDelta = Vector2.zero;
        duckTransform.sizeDelta = Vector2.zero;
    }

    public void ChangeScene(int sceneNum)
    {
        if (transitionActive) return;

        StartCoroutine(TransitionChangeScene(sceneNum));
    }

    public void ReloadScene()
    {
        ChangeScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Changes scenes to the specified index while doing a transition animation
    IEnumerator TransitionChangeScene(int index)
    {
        Pause_Menu.canPause = false;
        transitionActive = true;

        //Get the size of the canvas and make a new vector2 that will store the ending scale of the rectangle. Set it to match the rectangles aspect ratio
        Vector2 canvasScale = canvasTransform.sizeDelta;
        Vector2 endScale = new Vector2(aspectRatio.x, aspectRatio.y);

        //Calculate the multiplier on the x and the y to cover the screen
        float xMult = canvasScale.x / endScale.x;
        float yMult = canvasScale.y / endScale.y;

        //Set mult to the larger of the two multipliers
        float mult = 1;
        if (xMult > yMult) mult = xMult;
        else mult = yMult;

        //Scale the ending scale of the rectangle based on the above multiplier and additional scaling factor
        endScale.x = endScale.x * mult * scaleMult;
        endScale.y = endScale.y * mult * scaleMult;

        //Store the starting time and the timer of how long has passed
        float startTime = Time.time;
        float timer = 0;

        //Repeat while the animation time has not passed
        while (Time.time <= transitionStartTime + startTime)
        {
            //Store the time that has passed since the animation started
            timer = Time.time - startTime;

            //Calculate the point in the animation between 0 and 1 using the time, also applies easing
            float ease = EaseIn(timer / transitionStartTime, easePow);

            //Interpolate the rectangles scale between 0 and the end scale based on the easing and apply it
            float xScale = Mathf.Lerp(0, endScale.x, ease);
            float yScale = Mathf.Lerp(0, endScale.y, ease);
            transitionTransform.sizeDelta = new Vector2(xScale, yScale);

            //Interpolate the rectangles position between the startPosition and end position based on easing and apply it.
            //The start position and end position are multiplied by the canvas scale so they scale with the canvas.
            float xPos = Mathf.Lerp(startPos.x * canvasScale.x, endPos.x * canvasScale.x, ease);
            float yPos = Mathf.Lerp(startPos.y * canvasScale.y, endPos.y * canvasScale.y, ease);
            transitionTransform.anchoredPosition = new Vector2(xPos, yPos);

            //Calculate the amount the duck image needs to be multiplied. The multiplier is the current scale of the rectangle divided by its original scale.
            //Calculated on a different axis depending on which one is driving the rectangles scale.
            float duckMult;
            if (xMult > yMult) duckMult = xScale / aspectRatio.x;
            else duckMult = yScale / aspectRatio.y;

            //Update the ducks position to match the new scale
            UpdateDuck(duckMult);

            yield return null;
        }

        //Update the rectangles position and scale to it's ending values
        transitionTransform.sizeDelta = endScale;
        transitionTransform.anchoredPosition = new Vector2(endPos.x * canvasScale.x, endPos.y * canvasScale.y);

        //Using similar logic to inside the animation loop, update the duck image to its ending position
        float duckEndMult;
        if (xMult > yMult) duckEndMult = endScale.x / aspectRatio.x;
        else duckEndMult = endScale.y / aspectRatio.y;
        UpdateDuck(duckEndMult);

        //Load the scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index);

        //Wait until the scene is done loading
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Pause_Menu.canPause = false;

        //Wait an extra frame to ensure there is no sudden pop
        yield return null;

        //Store the starting time and the timer of how long has passed
        startTime = Time.time;
        timer = 0;

        //Repeat while the animation time has not passed
        while (Time.time <= transitionEndTime + startTime)
        {
            //Store the time that has passed since the animation started
            timer = Time.time - startTime;

            //Calculate the point in the animation between 0 and 1 using the time, also applies easing
            float ease = EaseOut(timer / transitionEndTime, easePow);

            //Interpolate the rectangles scale between 0 and the end scale based on the easing and apply it
            float xScale = Mathf.Lerp(endScale.x, 0, ease);
            float yScale = Mathf.Lerp(endScale.y, 0, ease);
            transitionTransform.sizeDelta = new Vector2(xScale, yScale);

            //Interpolate the rectangles position between the startPosition and end position based on easing and apply it.
            //The start position and end position are multiplied by the canvas scale so they scale with the canvas.
            float xPos = Mathf.Lerp(endPos.x * canvasScale.x, startPos.x * canvasScale.x, ease);
            float yPos = Mathf.Lerp(endPos.y * canvasScale.y, startPos.y * canvasScale.y, ease);
            transitionTransform.anchoredPosition = new Vector2(xPos, yPos);

            //Calculate the amount the duck image needs to be multiplied. The multiplier is the current scale of the rectangle divided by its original scale.
            //Calculated on a different axis depending on which one is driving the rectangles scale.
            float duckMult;
            if (xMult > yMult) duckMult = xScale / aspectRatio.x;
            else duckMult = yScale / aspectRatio.y;

            //Update the ducks position to match the new scale
            UpdateDuck(duckMult);

            yield return null;
        }

        //Set the rectangle and duck's position and scale to their starting positions. The scales are 0 so they can't be seen.
        transitionTransform.sizeDelta = Vector2.zero;
        transitionTransform.anchoredPosition = new Vector2(startPos.x * canvasScale.x, startPos.y * canvasScale.y);
        duckTransform.sizeDelta = Vector2.zero;

        Pause_Menu.canPause = true;
        transitionActive = false;
    }

    void UpdateDuck(float scale)
    {
        duckTransform.sizeDelta = new Vector2(aspectRatioDuck.x * scale, aspectRatioDuck.y * scale);
        Vector2 rectPos = transitionTransform.anchoredPosition;
        duckTransform.anchoredPosition = new Vector2(rectPos.x + offset.x * scale, rectPos.y + offset.y * scale);
    }

    float EaseIn(float t, int p)
    {
        return Mathf.Pow(t, p);
    }

    float Flip(float t)
    {
        return 1 - t;
    }

    float EaseOut(float t, int p)
    {
        return Flip(EaseIn(Flip(t), p));
    }
}
