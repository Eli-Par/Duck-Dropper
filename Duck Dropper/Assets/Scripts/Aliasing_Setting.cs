using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Aliasing_Setting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int aliasingLevel = PlayerPrefs.GetInt("aliasingLevel");

        if (aliasingLevel == 0) Camera.main.GetComponent<UniversalAdditionalCameraData>().antialiasing = AntialiasingMode.None;
        else if (aliasingLevel == 1) Camera.main.GetComponent<UniversalAdditionalCameraData>().antialiasing = AntialiasingMode.FastApproximateAntialiasing;
        else
        {
            Camera.main.GetComponent<UniversalAdditionalCameraData>().antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
            if (aliasingLevel == 2) Camera.main.GetComponent<UniversalAdditionalCameraData>().antialiasingQuality = AntialiasingQuality.Low;
            else if (aliasingLevel == 3) Camera.main.GetComponent<UniversalAdditionalCameraData>().antialiasingQuality = AntialiasingQuality.Medium;
            else if (aliasingLevel == 4) Camera.main.GetComponent<UniversalAdditionalCameraData>().antialiasingQuality = AntialiasingQuality.High;

        }
    }
}
