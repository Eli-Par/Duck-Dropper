using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duck_Physics : MonoBehaviour
{
    public float threshold;
    public float height;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        //Get the rigidbody of the duck
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //If the duck is not moving and is low enough down, stop all physics
        if(rb.velocity.magnitude < threshold && transform.position.y < height)
        {
            rb.isKinematic = true;
        }
    }
}
