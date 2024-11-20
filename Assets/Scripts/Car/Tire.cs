using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tire : MonoBehaviour
{
    [SerializeField]
    private RectRigidBody carRB;

    private Vector3 accForces = Vector3.zero;

    private RaycastHit tireRay;

    //suspension variables
    private Vector3 suspensionForce;
    public float suspensionRestLength = 0.5f;
    public float suspensionStrength = 10;
    public float suspensionDamping = 20;

    //Acceleration variables
    private Vector3 accelerationForce;
    public float topSpeed = 100;

    //Steering variables
    private Vector3 steeringForce;
    public float tireGripFactor = 1.0f;

    //UpdateForce(), shoots ray and if ray hits update everything
    //GetForce()

    private void Start()
    {
        suspensionForce = Vector3.zero;
        accelerationForce = Vector3.zero;
        steeringForce = Vector3.zero;
    }

    private void FixedUpdate()
    {
        //shoot ray downwards
        //If ray hits then update forces
    }

    public void UpdateForces(float _accelerationInput, Vector3 _carForward)
    {
        //UpdateSuspension();
        UpdateAcceleration(_accelerationInput, _carForward);
        UpdateSteering();

        accForces = suspensionForce + accelerationForce + (5 * steeringForce);
        Debug.DrawLine(transform.position, transform.position + accForces, Color.green);
    }

    public Vector3 GetForces()
    {
        return accForces;
    }

    private void UpdateSuspension() //UpdateSuspension, needs tire transform, springStrength, springRestDistance
    {
        //World space direction of suspension force
        Vector3 suspensionDirection = transform.up;

        Vector3 tireWorldVelocity = carRB.GetVelocityAtPoint(transform.position);

        //Offset of spring length and distance to floor
        float offset = suspensionRestLength - (suspensionRestLength - 0.01f) /*- rayDistance*/;

        //Velocity of tire moving up and down due to suspension
        float springVelocity = Vector3.Dot(suspensionDirection, tireWorldVelocity);

        float forceMagnitude = (offset * suspensionStrength) - (springVelocity * suspensionDamping);

        suspensionForce = suspensionDirection * forceMagnitude;
    }

    private void UpdateAcceleration(float _accelerationInput, Vector3 _carForward)
    {
        //World space direction of acceleration force
        Vector3 accelerationDirection = transform.forward;

        if (_accelerationInput > 0.0f)
        {
            float carSpeed = Vector3.Dot(_carForward, carRB.COM.velocity);

            //Speed as float 0-1 based on how close to top speed
            float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / topSpeed);

            float powerToTire = 1.0f * _accelerationInput; //change 1.0f to a lookup curve that uses normalizedSpeed

            accelerationForce = accelerationDirection * powerToTire;
        }
    }

    private void UpdateSteering() //UpdateSteering, needs tire transform, tireGripFactor 1 max grip 0 no grip
    {
        //World space direction of steering force
        Vector3 steeringDirection = transform.right;

        Vector3 tireWorldVelocity = carRB.GetVelocityAtPoint(transform.position);

        //Tires velocity in the steering direction
        float steeringVelocity = Vector3.Dot(steeringDirection, tireWorldVelocity);

        //Apply grip factor and negate to get the correct force to apply
        float deltaVelocity = -steeringVelocity * tireGripFactor;

        //Change to acceleration to change velocity of car by correct amount
        float deltaAcceleration = deltaVelocity/* / Time.fixedDeltaTime*/;

        steeringForce = steeringDirection * deltaAcceleration;
    }
}
