using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -10);
    private Vector3 offsetWorld;

    public float folllowSpeed = 10.0f;
    public float lookSpeed = 10.0f;
    
    // Update is called once per frame
    void FixedUpdate()
    {
        //Get offset in world space
        offsetWorld = target.localToWorldMatrix * offset;

        //Move Camera
        Vector3 desiredPos = target.position + offsetWorld;
        //Interpolate to desired position
        transform.position = Vector3.Lerp(transform.position, desiredPos, folllowSpeed * Time.fixedDeltaTime);

        //Orient Camera
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);

        //Interpolate camera movement
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, lookSpeed * Time.fixedDeltaTime);
    }
}
