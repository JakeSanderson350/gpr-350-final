using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBController : MonoBehaviour
{
    private RectRigidBody rb;

    private Vector3 dimensions;
    private Vector3 halfLengths;

    private Vector3 applicationPos;
    bool firstTime = true;

    void Start()
    {
        rb = GetComponent<RectRigidBody>();
        dimensions = rb.getDimensions();
        halfLengths = dimensions / 2;
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
            applicationPos = new Vector3(-halfLengths.x, -halfLengths.y, halfLengths.z);
            rb.AddForce(new Vector3(100, 0, 0), transform.position + applicationPos);
            Debug.DrawLine(transform.position + new Vector3(-halfLengths.x - 0.25f, -halfLengths.y, halfLengths.z), transform.position + applicationPos, Color.green, 3.0f);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            applicationPos = new Vector3(-halfLengths.x, halfLengths.y, -halfLengths.z);
            rb.AddForce(new Vector3(10, 0, 0), transform.position + applicationPos);
            Debug.DrawLine(transform.position + new Vector3(-halfLengths.x - 0.25f, halfLengths.y, -halfLengths.z), transform.position + applicationPos, Color.green, 3.0f);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            applicationPos = new Vector3(halfLengths.x, -halfLengths.y, halfLengths.z);
            rb.AddForce(new Vector3(0, 10, 0), transform.position + applicationPos);
            Debug.DrawLine(transform.position + new Vector3(halfLengths.x, -halfLengths.y - 0.25f, halfLengths.z), transform.position + applicationPos, Color.green, 3.0f);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            applicationPos = new Vector3(-halfLengths.x, -halfLengths.y, -halfLengths.z);
            rb.AddForce(new Vector3(0, 10, 0), transform.position + applicationPos);
            Debug.DrawLine(transform.position + new Vector3(-halfLengths.x, -halfLengths.y - 0.25f, -halfLengths.z), transform.position + applicationPos, Color.green, 3.0f);
        }
        if(firstTime)
        {
            applicationPos = new Vector3(-halfLengths.x, halfLengths.y, -halfLengths.z);
            rb.AddForce(new Vector3(10, 0, 0), transform.position + applicationPos);
            Debug.DrawLine(transform.position + new Vector3(-halfLengths.x - 0.25f, halfLengths.y, -halfLengths.z), transform.position + applicationPos, Color.green, 3.0f);
            firstTime = false;
        }
    }
}
