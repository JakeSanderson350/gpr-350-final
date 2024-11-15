using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBController : MonoBehaviour
{
    private RectRigidBody customRigidbody;

    void Start()
    {
        customRigidbody = GetComponent<RectRigidBody>();
    }

    void Update()
    {
        // Apply force with the space key at the center of the object
        if (Input.GetKeyDown(KeyCode.Space))
        {
            customRigidbody.AddForce(new Vector3(0, 10, 0), transform.position);
        }

        // Apply force at different points using arrow keys
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            customRigidbody.AddForce(new Vector3(0, 1, 0), transform.position + new Vector3(1, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            customRigidbody.AddForce(new Vector3(0, 1, 0), transform.position + new Vector3(-1, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            customRigidbody.AddForce(new Vector3(0, 1, 0), transform.position + new Vector3(0, 0, 1));
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            customRigidbody.AddForce(new Vector3(0, 1, 0), transform.position + new Vector3(0, 0, -1));
        }
    }
}
