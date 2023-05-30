using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Script : MonoBehaviour
{
    public GameObject obj;

    public Vector3 offset;

    public Vector3 reps;

    // Start is called before the first frame update
    void Start()
    {
        for(int x = 0; x < reps.x; x++)
        {
            for (int y = 0; y < reps.y; y++)
            {
                for (int z = 0; z < reps.z; z++)
                {
                    Vector3 pos = new Vector3(transform.position.x + offset.x * x, transform.position.y + offset.y * y, transform.position.z + offset.z * z);
                    GameObject temp = Instantiate(obj, pos, Quaternion.identity);
                    temp.transform.LookAt(Camera.main.transform.position);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
