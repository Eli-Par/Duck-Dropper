using System.Collections.Generic;
using UnityEngine;

public class Duck_Section : MonoBehaviour
{
    public int duckQualityCount = 4;
    public List<GameObject>[] duckQueues;

    public bool cullingEnabled = true;

    public float cullingOffset = 2f;

    public float minHeight = -20f;

    float highestDuck = -20f;

    public GameObject blockObj;

    public float blockHeight = 20f;

    GameObject block;

    public float blockY = -100;

    List<Duck_Section> adjacentSections = new List<Duck_Section>();

    [Space]
    public int[] counts = new int[4];

    void Start()
    {
        //Setup an array of lists that serve as queues for the different duck qualities
        duckQueues = new List<GameObject>[duckQualityCount];
        for(int i = 0; i < duckQueues.Length; i++)
        {
            duckQueues[i] = new List<GameObject>();
        }

        //Find the static queue game object and store a reference to it
        Static_Queue staticQueue = GameObject.FindGameObjectWithTag("StaticQueue").GetComponent<Static_Queue>();

        //Add each adjacent section to the list of adjacent sections
        AddAdjacentSection(staticQueue, new Vector2(1, 0));
        AddAdjacentSection(staticQueue, new Vector2(-1, 0));
        AddAdjacentSection(staticQueue, new Vector2(0, 1));
        AddAdjacentSection(staticQueue, new Vector2(0, -1));

        //Instantiate a yellow block for the section, scale it correctly and update it's position
        block = Instantiate(blockObj, transform.position, Quaternion.identity);
        block.transform.localScale = new Vector3(transform.localScale.x, blockHeight, transform.localScale.z);
        UpdateBlock(null, blockY);
    }

    void Update()
    {
        for(int i = 0; i < 4; i++)
        {
            counts[i] = duckQueues[i].Count;
        }
    }

    public void AddAdjacentSection(Static_Queue queue, Vector2 dir)
    {
        //Find the index of the section in the direction specified
        int index = queue.FindObjectIndex(transform.position.x + dir.x, transform.position.z + dir.y);

        //Check if the section is the back right corner, if so skip adding the section
        //(The back right corner section is out of the play space and it used for invalid locations)
        if(index == queue.sectionsList.Length - 1)
        {
            return;
        }

        //Find the corresponding section and add that section to the list of adjacent sections
        GameObject sectionObj = queue.sectionsList[index];
        Duck_Section section = sectionObj.GetComponent<Duck_Section>();
        adjacentSections.Add(section);
    }

    public void CheckCulling(GameObject newDuck, Static_Queue queue)
    {
        //If the new duck is higher than the highest duck so far, it's y position is the new highest
        if(highestDuck < newDuck.transform.position.y)
        {
            highestDuck = newDuck.transform.position.y;
            
            //Since the height has changed, have all adjacent sections check if their yellow block should move
            for(int i = 0; i < adjacentSections.Count; i++)
            {
                Duck_Section currSection = adjacentSections[i];
                currSection.CheckAdjacentSections(queue);
            }

        }
    }

    public void CheckAdjacentSections(Static_Queue queue)
    {
        //Loop through each adjacent section and find the lowest height that an adjacent section has
        float lowestAdjacentDuck = 1000;
        for (int i = 0; i < adjacentSections.Count; i++)
        {
            Duck_Section currSection = adjacentSections[i];
            if (currSection.highestDuck < lowestAdjacentDuck)
            {
                lowestAdjacentDuck = currSection.highestDuck;
            }

        }

        //If the lowest adjacent height minus the offset value is higher than the block, culling is enabled and the new height is above the minimum, update the block to move it up
        if (lowestAdjacentDuck - cullingOffset > blockY && cullingEnabled && lowestAdjacentDuck - cullingOffset >= minHeight)
        {
            blockY = lowestAdjacentDuck - cullingOffset;
            UpdateBlock(queue, blockY);
        }
    }

    public void UpdateBlock(Static_Queue queue, float height)
    {
        //Move the block so that the top of it is at the specified height
        block.transform.position = new Vector3(transform.position.x, height - blockHeight / 2, transform.position.z);

        //If no queue was specified, return
        if (queue == null) return;

        //Loop through each list of ducks in this section
        for (int i = 0; i < duckQualityCount; i++)
        {
            //Loop through all ducks in the list
            for (int k = 0; k < duckQueues[i].Count; k++)
            {
                //If the current duck is under the block, remove it from the list and add it to a queue of static ducks to be reused
                GameObject currDuck = duckQueues[i][k];
                if (currDuck.transform.position.y < height)
                {
                    duckQueues[i].RemoveAt(k);
                    queue.recycledDuckQueues[i].Enqueue(currDuck);
                    k--;
                }
            }
        }
    }
}