using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enable_On_Quality : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private int qualitySetting;
    [SerializeField] private bool activeOnQuality = false;

    // Start is called before the first frame update
    void Start()
    {
        if(QualitySettings.GetQualityLevel() == qualitySetting)
        {
            meshRenderer.enabled = activeOnQuality;
        }
        else
        {
            meshRenderer.enabled = !activeOnQuality;
        }
    }

}
