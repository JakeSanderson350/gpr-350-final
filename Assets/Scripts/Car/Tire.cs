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
    public float suspensionStrength = 600;
    public float suspensionDamping = 100.0f;

    //Acceleration variables
    private Vector3 accelerationForce;
    private Vector3 brakingForce;
    public float topSpeed = 100;
    public float brakeStrength = 0.5f;

    //Steering variables
    private Vector3 steeringForce;
    public float tireGripFactor = 1.0f;

    private void Start()
    {
        suspensionForce = Vector3.zero;
        accelerationForce = Vector3.zero;
        steeringForce = Vector3.zero;
    }

    public void UpdateForces(float _accelerationInput, float _brakeInput, Vector3 _carForward)
    {
        if (Physics.Raycast(transform.position, -transform.up, out tireRay, suspensionRestLength + 0.1f))
        {
            UpdateSuspension();
            UpdateAcceleration(_accelerationInput, _carForward);
            UpdateBraking(_brakeInput, _carForward);
            UpdateSteering();
        }

        accForces = suspensionForce + accelerationForce + brakingForce + steeringForce;
        Debug.DrawLine(transform.position, transform.position + accForces, Color.green);

        suspensionForce = Vector3.zero;
        accelerationForce = Vector3.zero;
        brakingForce = Vector3.zero;
        steeringForce = Vector3.zero;
    }

    public Vector3 GetForces()
    {
        return accForces;
    }

    private void UpdateSuspension() //UpdateSuspension, needs tire transform, springStrength, springRestDistance
    {
        //World space direction of suspension force
        Vector3 suspensionDirection = transform.up;

        //Vector3 tireWorldVelocity = carRB.GetVelocityAtPoint(transform.position);
        Vector3 tireWorldVelocity = carRB.COM.velocity; //Only works with linear velocity no angular?

        //Offset of spring length and distance to floor
        float offset = suspensionRestLength - tireRay.distance;

        //Velocity of tire moving up and down due to suspension
        float springVelocity = Vector3.Dot(suspensionDirection, tireWorldVelocity);

        float forceMagnitude = (offset * suspensionStrength) - (springVelocity * suspensionDamping);

        if (forceMagnitude > 0)
        {
            suspensionForce = suspensionDirection * forceMagnitude;
        }
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

    private void UpdateBraking(float _brakingInput, Vector3 _carForward)
    {
        //World space direction of braking force
        Vector3 brakingDirection = -transform.forward;

        if (_brakingInput > 0.0f)
        {
            float powerToTire = brakeStrength * _brakingInput; //change 1.0f to a lookup curve that uses normalizedSpeed

            brakingForce = brakingDirection * powerToTire;
        }

        //if (_brakingInput > 0.0f)
        //{
        //    Vector3 brakingDirection = transform.forward;

        //    Vector3 tireWorldVelocity = carRB.GetVelocityAtPoint(transform.position);

        //    //Tires velocity in the braking direction
        //    float brakingVelocity = Vector3.Dot(brakingDirection, tireWorldVelocity);

        //    //Apply grip factor and negate to get the correct force to apply
        //    float deltaVelocity = -brakingVelocity * brakeStrength;

        //    brakingForce = brakingDirection * deltaVelocity;
        //}
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

        steeringForce = steeringDirection * deltaVelocity;
    }
}
