using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCameraController : MonoBehaviour
{
    public Transform target;
    private Vector3 targetPosiition;
    public Vector3 thirdPersonOffset = new Vector3(0, 7, -2.5f);
    public Vector3 firstPersonOffset = new Vector3(-0.25f, 1.0f, 0.0f);
    private Vector3 offsetWorld;

    public float folllowSpeed = 10.0f;
    public float lookSpeed = 10.0f;

    public bool isThirdPerson = true;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isThirdPerson = !isThirdPerson;
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    { 
        Vector3 offset = isThirdPerson ? thirdPersonOffset : firstPersonOffset;

        //Get offset in world space
        offsetWorld = target.TransformDirection(offset);

        //Move Camera
        Vector3 desiredPos = target.position + offsetWorld;
        //Interpolate to desired position
        transform.position = Vector3.Lerp(transform.position, desiredPos, folllowSpeed * Time.fixedDeltaTime);

        //Orient Camera
        if (isThirdPerson)
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);

            //Interpolate camera movement
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, lookSpeed * Time.fixedDeltaTime);
        }
        else
        {
            //If first person align camera with target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, lookSpeed * Time.fixedDeltaTime);
        }
    }
}
