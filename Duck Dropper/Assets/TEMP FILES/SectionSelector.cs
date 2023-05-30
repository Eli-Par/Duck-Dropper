using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionSelector : MonoBehaviour
{
    public GameObject[] objs;
    public int width;
    public int height;

    public Vector2 leftCorner;

    public GameObject target;

    public Material m1;
    public Material m2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float targetX = target.transform.position.x;
        float targetY = target.transform.position.z;

        int index = FindObjectIndex(targetX, targetY);

        for (int i = 0; i < objs.Length; i++)
        {
            objs[i].GetComponent<MeshRenderer>().material = m1;
        }

        objs[index].GetComponent<MeshRenderer>().material = m2;

    }

    //Takes an x and y position (in 3d x and z). Returns the index of the nearest section object
    int FindObjectIndex(float objX, float objY)
    {
        //Calculate x and y indices
        int xIndex = (int)Mathf.Round(-objX + leftCorner.x);
        int yIndex = (int)Mathf.Round(-objY + leftCorner.y);

        //Check for out of bounds indices
        if (xIndex < 0)
        {
            Debug.LogWarning("Section selection  out of range: xIndex = " + xIndex);
            xIndex = 0;
        }
        if (xIndex >= width)
        {
            Debug.LogWarning("Section selection  out of range: xIndex = " + xIndex);
            xIndex = width - 1;
        }
        if (yIndex < 0)
        {
            Debug.LogWarning("Section selection  out of range: yIndex = " + yIndex);
            yIndex = 0;
        }
        if (yIndex >= height)
        {
            Debug.LogWarning("Section selection  out of range: yIndex = " + yIndex);
            yIndex = height - 1;
        }

        //Calculate array index using location on grid
        int objectIndex = yIndex * width + xIndex;

        return objectIndex;
    } //end FindObjectIndex
}
