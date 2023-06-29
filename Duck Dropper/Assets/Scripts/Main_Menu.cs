using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;

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

    [Space]
    public int defaultQualityLevel = 2;
    public int qualityLevel = 0;
    public string qualityLevelKey = "qualityLevel";
    public TextMeshProUGUI qualityLevelText = default;
    public string qualityLevelPrefix = "Quality: ";

    [Space]
    public int aliasingDefault = 0;
    public int aliasingLevel = 0;
    public string aliasingKey = "aliasingLevel";
    public TextMeshProUGUI aliasingText = default;
    public string aliasingTextPrefix = "Quality: ";
    public string[] aliasingNames = default;
    public RectTransform aliasingButtonTransform;
    public float aliasingNormalWidth = default;
    public float aliasingLargeWidth = default;

    [Space]
    public int duckLevelDefault = 0;
    public int duckLevel = 0;
    public string duckLevelKey = "duckLevel";
    public TextMeshProUGUI duckLevelText = default;
    public string duckLevelTextPrefix = "Duck Quality: ";
    public string[] duckLevelNames = default;

    // Start is called before the first frame update
    void Start()
    {
        //If a quality level exists, load the setting. Otherwise set it to the default
        if(PlayerPrefs.HasKey(qualityLevelKey))
        {
            qualityLevel = PlayerPrefs.GetInt(qualityLevelKey);
        }
        else
        {
            PlayerPrefs.SetInt(qualityLevelKey, defaultQualityLevel);
            qualityLevel = defaultQualityLevel;
        }

        //Set the quality setting and text to match
        QualitySettings.SetQualityLevel(qualityLevel);
        qualityLevelText.text = qualityLevelPrefix + QualitySettings.names[qualityLevel];

        //If a aliasing level exists, load the setting. Otherwise set it to the default
        if (PlayerPrefs.HasKey(aliasingKey))
        {
            aliasingLevel = PlayerPrefs.GetInt(aliasingKey);
        }
        else
        {
            PlayerPrefs.SetInt(aliasingKey, aliasingDefault);
            aliasingLevel = aliasingDefault;
        }

        //Set the aliasing setting and text to match
        SetAliasingQuality();
        aliasingText.text = aliasingTextPrefix + aliasingNames[aliasingLevel];

        //
        if (PlayerPrefs.HasKey(duckLevelKey))
        {
            duckLevel = PlayerPrefs.GetInt(duckLevelKey);
        }
        else
        {
            PlayerPrefs.SetInt(duckLevelKey, duckLevelDefault);
            duckLevel = duckLevelDefault;
        }

        //
        duckLevelText.text = duckLevelTextPrefix + duckLevelNames[duckLevel];
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

    public void IncreaseQualityLevel()
    {
        //Get an array of all quality names
        string[] qualityNames = QualitySettings.names;

        //Increase the quality and make it go back to the minimum if out of bounds
        qualityLevel++;
        if (qualityLevel >= qualityNames.Length) qualityLevel = 0;

        //Change the quality setting and text
        QualitySettings.SetQualityLevel(qualityLevel);
        qualityLevelText.text = qualityLevelPrefix + qualityNames[qualityLevel];

        //Store the new setting
        PlayerPrefs.SetInt(qualityLevelKey, qualityLevel);
    }

    public void IncreaseAliasingLevel()
    {
        //
        aliasingLevel++;
        if (aliasingLevel >= aliasingNames.Length) aliasingLevel = 0;

        //
        SetAliasingQuality();
        aliasingText.text = aliasingTextPrefix + aliasingNames[aliasingLevel];

        //
        PlayerPrefs.SetInt(aliasingKey, aliasingLevel);
    }

    private void SetAliasingQuality()
    {
        if (aliasingLevel == 0) Camera.main.GetComponent<UniversalAdditionalCameraData>().antialiasing = AntialiasingMode.None;
        else if (aliasingLevel == 1) Camera.main.GetComponent<UniversalAdditionalCameraData>().antialiasing = AntialiasingMode.FastApproximateAntialiasing;
        else
        {
            Camera.main.GetComponent<UniversalAdditionalCameraData>().antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
            if (aliasingLevel == 2) Camera.main.GetComponent<UniversalAdditionalCameraData>().antialiasingQuality = AntialiasingQuality.Low;
            else if (aliasingLevel == 3) Camera.main.GetComponent<UniversalAdditionalCameraData>().antialiasingQuality = AntialiasingQuality.Medium;
            else if (aliasingLevel == 4) Camera.main.GetComponent<UniversalAdditionalCameraData>().antialiasingQuality = AntialiasingQuality.High;

        }

        if(aliasingLevel <= 1)
        {
            aliasingButtonTransform.sizeDelta = new Vector2(aliasingNormalWidth, aliasingButtonTransform.sizeDelta.y);
        }
        else
        {
            aliasingButtonTransform.sizeDelta = new Vector2(aliasingLargeWidth, aliasingButtonTransform.sizeDelta.y);
        }

    }

    public void IncreaseDuckLevel()
    {
        //
        duckLevel++;
        if (duckLevel >= duckLevelNames.Length) duckLevel = 0;

        //
        duckLevelText.text = duckLevelTextPrefix + duckLevelNames[duckLevel];

        //
        PlayerPrefs.SetInt(duckLevelKey, duckLevel);
    }

}
