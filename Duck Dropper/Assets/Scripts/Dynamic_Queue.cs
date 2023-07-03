using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dynamic_Queue : MonoBehaviour
{
    [Header("High Quality Ducks")]
    [SerializeField] private GameObject duckObj = default;
    [SerializeField] private int duckCount = 150;

    [Header("Basic Ducks")]
    [SerializeField] private GameObject basicDuckObj = default;
    [SerializeField] private int basicDuckCount = 150;

    [Header("Static Queue")]
    [SerializeField] private Static_Queue staticQueue = default;

    [Header("Quality Switching Speed Range")]
    [SerializeField] private float verticalSpeedSwapMax = 0.05f;
    [SerializeField] private float verticalSpeedSwapMin = 0.05f;

    GameObject[] duckList;
    GameObject[] basicDuckList;

    int duckIndex = 0;
    int basicDuckIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        duckList = new GameObject[duckCount];
        basicDuckList = new GameObject[basicDuckCount];
    }

    //Load maximum duck counts and have the status queue do the same
    public void SetDuckSettings(Duck_Setting setting)
    {
        duckCount = setting.dynamicHighDuckCount;
        basicDuckCount = setting.dynamicDuckCount;

        staticQueue.SetDuckSettings(setting);
    }

    public void AddDuck(Vector3 pos)
    {
        //If there is not already a high quality duck in the list, add one.
        //Otherwise, move a basic duck to replace a high quality duck and use the replaced high quality duck
        if(duckList[duckIndex] == null)
        {
            duckList[duckIndex] = Instantiate(duckObj, pos, Random.rotation);
        }
        else
        {
            //Get the quality level that the static queue can provide
            int staticQuality = staticQueue.QualityAtLocation(duckList[duckIndex].transform.position);

            //Store the rigidbody of the current high quality duck for future use
            Rigidbody currDuckRB = duckList[duckIndex].GetComponent<Rigidbody>();

            //If the static queue can provide a high quality duck and the current dynamic high quality duck y velocity is within a specified range, have the static queue replace the duck, otherwise replace it with the dynamic queue
            if (staticQuality == 0 && currDuckRB.velocity.y < verticalSpeedSwapMax && currDuckRB.velocity.y > verticalSpeedSwapMin && duckList[duckIndex].transform.position.y < 5)  // duckList[duckIndex].GetComponent<Rigidbody>().velocity.magnitude < 0.05f && duckList[duckIndex].transform.position.y < 5
            {
                staticQueue.AddDuck(staticQuality, duckList[duckIndex].transform.position, duckList[duckIndex].transform.rotation);

                duckList[duckIndex].transform.position = pos;
                currDuckRB.velocity = Vector3.zero;
            }
            else
            {
                //If there isn't already a basic duck, add one.
                if (basicDuckList[basicDuckIndex] == null)
                {
                    basicDuckList[basicDuckIndex] = Instantiate(basicDuckObj, duckList[duckIndex].transform.position, duckList[duckIndex].transform.rotation);
                }
                else
                {
                    Vector3 basicDuckPosition = basicDuckList[basicDuckIndex].transform.position;

                    if (basicDuckList[basicDuckIndex].GetComponent<Rigidbody>().velocity.magnitude >= 0.05f)
                    {
                        Debug.LogWarning("Duck switched with velocity");

                        RaycastHit hit;
                        if (Physics.Raycast(basicDuckPosition, -Vector3.up, out hit))
                        {
                            basicDuckPosition = hit.point;
                        }
                    }

                    //If the basic duck is being reused, replace it with a static duck
                    staticQueue.AddDuck(staticQueue.QualityAtLocation(basicDuckList[basicDuckIndex].transform.position), basicDuckPosition, basicDuckList[basicDuckIndex].transform.rotation);
                }

                //Swap the basic duck and high quality duck
                SwapDuckIndexes(pos, currDuckRB);

                //Increment basicDuckIndex, if out of bounds set index to 0
                basicDuckIndex++;
                if (basicDuckIndex >= basicDuckList.Length) basicDuckIndex = 0;
            }
        }

        //Increment duckIndex, if out of bounds set index to 0
        duckIndex++;
        if (duckIndex >= duckList.Length) duckIndex = 0;

    }

    //Moved the basic duck to the high quality ducks old position and moves the high quality duck to a new position.
    private void SwapDuckIndexes(Vector3 newPos, Rigidbody duckRB)
    {
        //Set the basic ducks position and rotation to be the same as the high quality duck
        basicDuckList[basicDuckIndex].transform.position = duckList[duckIndex].transform.position;
        basicDuckList[basicDuckIndex].transform.rotation = duckList[duckIndex].transform.rotation;

        //Set the basic ducks velocity to match the high quality duck
        Rigidbody basicRB = basicDuckList[basicDuckIndex].GetComponent<Rigidbody>();
        //Rigidbody duckRB = duckList[duckIndex].GetComponent<Rigidbody>();
        basicRB.isKinematic = false;
        basicRB.velocity = duckRB.velocity;
        basicRB.angularVelocity = duckRB.angularVelocity;

        //Move the high quality ducks location to the new position and remove all velocity.
        duckList[duckIndex].transform.position = newPos;
        duckRB.velocity = Vector3.zero;

    }
}
