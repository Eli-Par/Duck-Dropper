using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Static_Queue : MonoBehaviour
{
    [Header("Section Grid")]
    public GameObject[] sectionsList;
    public int[] sectionIndices;
    public int width = 1;
    public int height = 1;

    [SerializeField] private Vector2 leftCorner = default;

    [Space]

    [Header("Duck Qualities")]
    [SerializeField] private GameObject[] duckObjs = default;
    [SerializeField] private int[] maxDucks = default;

    private int[] currentDuckCounts;

    [Space]

    [Header("Screen Boundary Offsets")]
    [SerializeField] private float xScreenOffset = 0.15f;
    [SerializeField] private float yScreenOffset = 0.25f;

    [System.NonSerialized]
    public Queue<GameObject>[] recycledDuckQueues;

    // Start is called before the first frame update
    void Start()
    {
        currentDuckCounts = new int[maxDucks.Length];
        recycledDuckQueues = new Queue<GameObject>[maxDucks.Length];

        //Set all the count of how many ducks of each quality exist to 0
        for (int i = 0; i < maxDucks.Length; i++)
        {
            currentDuckCounts[i] = 0;
        }

        for(int i = 0; i < recycledDuckQueues.Length; i++)
        {
            recycledDuckQueues[i] = new Queue<GameObject>();
        }
    }

    void Update()
    {

    }

    //Load maximum duck counts
    public void SetDuckSettings(Duck_Setting setting)
    {
        maxDucks[0] = setting.staticHighDuckCount;
        maxDucks[1] = setting.staticDuckCount;
        maxDucks[2] = setting.staticBoxDuckCount;
        maxDucks[3] = setting.staticFlatDuckCount;
    }

    //Returns true if the position is on screen with a buffer around the edges
    private bool PointOnScreen(Vector3 pos)
    {
        //Convert position to screen position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);

        //Calculate value between 0 and 1 for the screen position (instead of 0 to width/height)
        float xVal = screenPos.x / Camera.main.pixelWidth;
        float yVal = screenPos.y / Camera.main.pixelHeight;

        //Check if the position is within the boundaries of the screen, with offsets providing a buffer around the edges of the screen
        return xVal >= 0 - xScreenOffset && xVal <= 1 + xScreenOffset && yVal >= 0 - yScreenOffset && yVal <= 1 + yScreenOffset;
    }

    public int QualityAtLocation(Vector3 position)
    {
        //Find the index of the section that contains the given position
        int index = FindObjectIndex(position.x, position.z);

        //Loop through all duck qualities
        for(int i = 0; i < duckObjs.Length; i++)
        {
            //If there isn't the maximum of the type of duck yet, return that quality level
            if (currentDuckCounts[i] < maxDucks[i])
            {
                return i;
            }

            //If the index is less than or at the section that contains the quality, return that index
            if (index <= FindHighestQualityIndex(i))
            {
                return i;
            }
        }

        //If no quality level is found, something has broken and return -1
        return -1;
    }

    //Add a duck to the screen at the given quality, position and rotation. If there is already the max of that quality, move a duck from farther back
    //to the new position so that higher quality ducks are in front.
    public void AddDuck(int quality, Vector3 position, Quaternion rotation)
    {
        //If the quality is out of range, send a warning
        if(quality >= duckObjs.Length)
        {
            Debug.LogWarning("Out of range duck quality: " + quality);
            return;
        }

        //If the position is off screen, add a very low quality duck and return
        if(!PointOnScreen(position))
        {
            GameObject newDuck = Instantiate(duckObjs[3], position, rotation);
            newDuck.transform.LookAt(Camera.main.transform.position);
            return;
        }

        if(recycledDuckQueues[quality].Count > 0)
        {
            //Debug.Log("Recycled Duck");

            GameObject newDuck = recycledDuckQueues[quality].Dequeue();
            newDuck.transform.position = position;
            newDuck.transform.rotation = rotation;

            RegisterDuck(newDuck, quality);
            return;
        }

        //If there isn't the maximum number of ducks of this quality or it's the last quality level, add a brand new one
        if (currentDuckCounts[quality] < maxDucks[quality] || quality == duckObjs.Length - 1)
        {
            //Instantiate a new duck of the given quality and increase the count for the number of ducks of that quality
            GameObject newDuck = Instantiate(duckObjs[quality], position, rotation);
            currentDuckCounts[quality]++;

            //Register the duck with the section it is in
            RegisterDuck(newDuck, quality);

            //If the duck is a flat duck, make it look at the camera
            if (quality == duckObjs.Length - 1)
            {
                newDuck.transform.LookAt(Camera.main.transform.position);
            }

            return;
        }
        else
        {
            //Get a duck of the given quality that will go to the new position
            GameObject currDuck = RepoDuck(quality, position);

            //Recursively add a duck to the position of the duck that is being added so there is still a duck there after it is moved
            AddDuck(quality + 1, currDuck.transform.position, currDuck.transform.rotation);

            //Change the ducks position and rotation so it's in the new position
            currDuck.transform.position = position;
            currDuck.transform.rotation = rotation;

            //Register the duck with it's section
            RegisterDuck(currDuck, quality);
        }
    }

    private GameObject RepoDuck(int quality, Vector3 futurePos)
    {
        //Duck_Section section = sectionsList[duckQualityIndex[quality]].GetComponent<Duck_Section>();

        //Get the section that is farthest back with a duck of the required quality
        Duck_Section section = sectionsList[FindHighestQualityIndex(quality)].GetComponent<Duck_Section>();

        //Warn if there is no duck to reposess
        if(section.duckQueues[quality].Count == 0)
        {
            Debug.LogWarning("No duck found to reposess of quality: " + quality);
            return null;
        }

        //Get the oldest duck from the section and remove it from the section
        GameObject duck = section.duckQueues[quality][0];
        section.duckQueues[quality].RemoveAt(0);

        //Return the found duck
        return duck;

    }

    //Adds the duck to the correct section
    private void RegisterDuck(GameObject duck, int quality)
    {
        //Get the index of the section the duck is in
        int sectionIndex = FindObjectIndex(duck.transform.position.x, duck.transform.position.z);

        //Get the GameObject for that section and store a reference to the script on the section
        GameObject section = sectionsList[sectionIndex];
        Duck_Section duckSection = section.GetComponent<Duck_Section>();

        //Add the duck to the end of the list of ducks of the given quality that are in that section
        duckSection.duckQueues[quality].Add(duck);

        duckSection.CheckCulling(duck, this);

    }

    //Returns the highest index that has a duck of a given quality
    private int FindHighestQualityIndex(int quality)
    {
        //Loop through all sections, starting at the ones with the largest indexes (farthest back)
        for (int i = sectionsList.Length - 1; i >= 0; i--)
        {
            //If the section contains a duck of the specified quality, return it's index
            if (sectionsList[i].GetComponent<Duck_Section>().duckQueues[quality].Count > 0)
            {
                return i;
            }
        }

        //If no section was found, return -1
        return -1;
    }

    //Takes an x and y position (in 3d x and z). Returns the index of the nearest section object
    public int FindObjectIndex(float objX, float objY)
    {
        //Calculate x and y indices
        int xIndex = (int) Mathf.Round(-objX + leftCorner.x);
        int yIndex = (int) Mathf.Round(-objY + leftCorner.y);

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

        return sectionIndices[objectIndex];
    } //end FindObjectIndex
}
