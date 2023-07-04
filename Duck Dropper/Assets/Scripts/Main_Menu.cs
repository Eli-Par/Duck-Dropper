using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.Universal;

public class Main_Menu : MonoBehaviour
{
    public int gameSceneIndex = 1;

    [Space]
    [Header("Instruction Menu")]
    public RectTransform canvasTransform;
    public RectTransform instructionTransform;

    [Space]
    [Header("Menu Transition")]
    public RectTransform menusTransform;
    public float screenOffset = 600;
    public int currentOffsetMultiplier = 0;
    public float screenTransitionSpeed = 600;

    [Space]
    [Header("Quality Settings")]
    public int defaultQualityLevel = 2;
    public int qualityLevel = 0;
    public string qualityLevelKey = "qualityLevel";
    public TextMeshProUGUI qualityLevelText = default;
    public string qualityLevelPrefix = "Quality: ";

    [SerializeField] private Enable_On_Quality enableOnQuality = default;

    [Space]
    [Header("Anti-aliasing Settings")]
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
    [Header("Duck Quality Settings")]
    public int duckLevelDefault = 0;
    public int duckLevel = 0;
    public string duckLevelKey = "duckLevel";
    public TextMeshProUGUI duckLevelText = default;
    public string duckLevelTextPrefix = "Duck Quality: ";
    public string[] duckLevelNames = default;

    [Space]
    [Header("Audio Settings")]
    [SerializeField] private TextMeshProUGUI musicVolumeText = default;
    [SerializeField] private string musicTextPrefix = default;
    [SerializeField] private Slider musicVolumeSlider = default;

    [SerializeField] private TextMeshProUGUI soundVolumeText = default;
    [SerializeField] private string soundTextPrefix = default;
    [SerializeField] private Slider soundVolumeSlider = default;

    [SerializeField] private string musicVolumeKey = "musicVolume";
    [SerializeField] private string soundVolumeKey = "soundVolume";

    private AudioSource musicSource;

    [Space]
    [Header("Camera Transition")]
    public float camRot1 = 0;
    public float camRot2 = 0;
    public int cameraEasePow = 3;

    [Space]
    [Header("Audio")]
    [SerializeField] private AudioSource clickAudio = default;

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

        //If an aliasing level exists, load the setting. Otherwise set it to the default
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

        //If a duck quality level exists, load the setting. Otherwise set it to the default
        if (PlayerPrefs.HasKey(duckLevelKey))
        {
            duckLevel = PlayerPrefs.GetInt(duckLevelKey);
        }
        else
        {
            PlayerPrefs.SetInt(duckLevelKey, duckLevelDefault);
            duckLevel = duckLevelDefault;
        }

        //Set the text to match the duck quality level
        duckLevelText.text = duckLevelTextPrefix + duckLevelNames[duckLevel];


        musicSource = GameObject.FindGameObjectWithTag("MusicPlayer").GetComponent<AudioSource>();

        if (PlayerPrefs.HasKey(musicVolumeKey))
        {
            musicVolumeSlider.value = PlayerPrefs.GetFloat(musicVolumeKey) * 20;
        }
        else
        {
            PlayerPrefs.SetFloat(musicVolumeKey, 1);
            musicVolumeSlider.value = 1;
        }

        if (PlayerPrefs.HasKey(soundVolumeKey))
        {
            soundVolumeSlider.value = PlayerPrefs.GetFloat(soundVolumeKey) * 20;
        }
        else
        {
            PlayerPrefs.SetFloat(soundVolumeKey, 1);
            soundVolumeSlider.value = 20;
        }

        MusicSliderChanged();
        SoundSliderChanged();
    }

    // Update is called once per frame
    void Update()
    {
        //If escape is pressed, reset to the main screen
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(currentOffsetMultiplier == 2)
            {
                SetScreenMultiplier(1);
            }
            else
            {
                SetScreenMultiplier(0);
            }
        }

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

        //If the menu is between the main menu and the options menu, rotate the camera
        if(yPos >= 0)
        {
            //Calculate the eased value to interpolate between rotations using the position the menu is between the two screens
            float easeInput = Mathf.Min(1, yPos / screenOffset);
            float ease = EaseInOut(easeInput, cameraEasePow);

            //Interpolate the rotation based on the easing
            float rot = Mathf.Lerp(camRot1, camRot2, ease);
            Debug.Log(yPos / screenOffset);

            //Rotate the camera to match the new rotation
            Transform camTransform = Camera.main.transform;
            camTransform.eulerAngles = new Vector3(camTransform.eulerAngles.x, rot, camTransform.eulerAngles.z);
        }

    }

    float EaseInOut(float t, int p)
    {
        return Mathf.Lerp(EaseIn(t, p), Flip(EaseIn(Flip(t), p)), t);
    }

    float Flip(float t)
    {
        return 1 - t;
    }

    float EaseIn(float t, int p)
    {
        return Mathf.Pow(t, p);
    }

    public void PlayClick()
    {
        clickAudio.Play();
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

        enableOnQuality.QualityUpdated();
    }

    public void IncreaseAliasingLevel()
    {
        //Increase the aliasing level and make it go back to the minimum if out of bounds
        aliasingLevel++;
        if (aliasingLevel >= aliasingNames.Length) aliasingLevel = 0;

        //Change the aliasing setting and text
        SetAliasingQuality();
        aliasingText.text = aliasingTextPrefix + aliasingNames[aliasingLevel];

        //Store the new setting
        PlayerPrefs.SetInt(aliasingKey, aliasingLevel);
    }

    private void SetAliasingQuality()
    {
        //Change anti-aliasing setting based on aliasingLevel
        if (aliasingLevel == 0) Camera.main.GetComponent<UniversalAdditionalCameraData>().antialiasing = AntialiasingMode.None;
        else if (aliasingLevel == 1) Camera.main.GetComponent<UniversalAdditionalCameraData>().antialiasing = AntialiasingMode.FastApproximateAntialiasing;
        else
        {
            Camera.main.GetComponent<UniversalAdditionalCameraData>().antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;

            //Change the anti-aliasing quality
            if (aliasingLevel == 2) Camera.main.GetComponent<UniversalAdditionalCameraData>().antialiasingQuality = AntialiasingQuality.Low;
            else if (aliasingLevel == 3) Camera.main.GetComponent<UniversalAdditionalCameraData>().antialiasingQuality = AntialiasingQuality.Medium;
            else if (aliasingLevel == 4) Camera.main.GetComponent<UniversalAdditionalCameraData>().antialiasingQuality = AntialiasingQuality.High;

        }

        //Change button width based on text size
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
        //Increase the duck quality level and make it go back to the minimum if out of bounds
        duckLevel++;
        if (duckLevel >= duckLevelNames.Length) duckLevel = 0;

        //Change the duck quality level text
        duckLevelText.text = duckLevelTextPrefix + duckLevelNames[duckLevel];

        //Store the new setting
        PlayerPrefs.SetInt(duckLevelKey, duckLevel);
    }

    public void MusicSliderChanged()
    {
        float volume = musicVolumeSlider.value * 0.05f;
        musicVolumeText.text = musicTextPrefix + (volume * 100).ToString("000") + "%";

        musicSource.volume = volume;

        PlayerPrefs.SetFloat(musicVolumeKey, volume);
    }

    public void SoundSliderChanged()
    {
        float volume = soundVolumeSlider.value * 0.05f;
        soundVolumeText.text = soundTextPrefix + (volume * 100).ToString("000") + "%";

        clickAudio.volume = volume;

        PlayerPrefs.SetFloat(soundVolumeKey, volume);
    }

}
