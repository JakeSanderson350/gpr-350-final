using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBController : MonoBehaviour
{
    private RectRigidBody rb;

    void Start()
    {
        rb = GetComponent<RectRigidBody>();
    }

    void Update()
    {
        // Apply force with the space key at the center of the object
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(new Vector3(0, 10, 0), transform.position + new Vector3(0, -0.5f, 0));
        }

        // Apply force at different points using arrow keys
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            rb.AddForce(new Vector3(10, 0, 0), transform.position + new Vector3(-1.25f, -0.5f, 2.5f));
            Debug.DrawLine(transform.position + new Vector3(-1.5f, -0.5f, 2.5f), transform.position + new Vector3(-1.25f, -0.5f, 2.5f), Color.green, 3.0f);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            rb.AddForce(new Vector3(0, 1, 0), transform.position + new Vector3(-1.125f, -0.5f, 0));
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            rb.AddForce(new Vector3(0, 1, 0), transform.position + new Vector3(0, -0.5f, 1.7f));
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            rb.AddForce(new Vector3(0, 1, 0), transform.position + new Vector3(0, 0, -1.7f));
        }
    }
}
