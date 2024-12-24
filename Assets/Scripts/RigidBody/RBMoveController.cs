using CodiceApp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBMoveController : MonoBehaviour
{
    private RectRigidBody rb;

    private Vector3 moveDirection = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<RectRigidBody>();
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.I))
        {
            moveDirection += new Vector3(1, 0, 0);
        }
        if (Input.GetKey(KeyCode.K))
        {
            moveDirection += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.J))
        {
            moveDirection += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.L))
        {
            moveDirection += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.U))
        {
            moveDirection += new Vector3(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.O))
        {
            moveDirection += new Vector3(0, -1, 0);
        }

        rb.COM.transform.position += moveDirection * Time.deltaTime;
    }
}
