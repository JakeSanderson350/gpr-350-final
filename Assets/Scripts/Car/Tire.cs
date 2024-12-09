using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tire : MonoBehaviour
{
    [SerializeField]
    private RectRigidBody carRB;

    private Vector3 accForces = Vector3.zero;

    //Collision data
    private RaycastHit tireRay;
    [SerializeField]
    public Sphere sphereCollider;

    //suspension variables
    private Vector3 suspensionForce;
    public float suspensionRestLength = 0.5f;
    public float suspensionStrength = 600;
    public float suspensionDamping = 100.0f;

    //Acceleration variables
    private Vector3 accelerationForce;
    public float topSpeed = 100;
    public float brakeStrength = 0.8f;
    private float frictionStrength = 0.3f;

    //Steering variables
    private Vector3 steeringForce;
    public float tireGripFactor = 0.5f;

    private void Start()
    {
        sphereCollider = FindObjectOfType<Sphere>();
        sphereCollider.Radius = 0.2f;
        sphereCollider.invMass = 0.1f; //mass of 10

        suspensionForce = Vector3.zero;
        accelerationForce = Vector3.zero;
        steeringForce = Vector3.zero;
    }

    public void UpdateForces(float _accelerationInput, float _brakeInput, Vector3 _carForward)
    {
        if (Physics.Raycast(transform.position, -transform.up, out tireRay, suspensionRestLength + 0.1f))
        {
            UpdateSuspension();
            UpdateAcceleration(_accelerationInput, _brakeInput, _carForward);
            UpdateSteering();
        }

        accForces = suspensionForce + accelerationForce + steeringForce;
        Debug.DrawLine(transform.position, transform.position + suspensionForce, Color.green);
        Debug.DrawLine(transform.position, transform.position + accelerationForce, Color.blue);
        Debug.DrawLine(transform.position, transform.position + steeringForce, Color.red);

        suspensionForce = Vector3.zero;
        accelerationForce = Vector3.zero;
        steeringForce = Vector3.zero;
    }

    public Vector3 GetForces()
    {
        return accForces;
    }

    private void UpdateSuspension()
    {
        //World space direction of suspension force
        Vector3 suspensionDirection = transform.up;

        Vector3 tireWorldVelocity = carRB.COM.velocity;

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

    private void UpdateAcceleration(float _accelerationInput, float _brakingInput, Vector3 _carForward)
    {
        //World space direction of acceleration force
        Vector3 accelerationDirection = transform.forward;

        //Projects linear velocity on forward vector to get speed of car
        float carSpeed = Vector3.Dot(_carForward, carRB.COM.velocity);

        //Acceleration force
        if (_accelerationInput > 0.0f)
        {
            //Speed as float 0-1 based on how close to top speed
            float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / topSpeed);

            float powerToTire = 1.0f * _accelerationInput; //change 1.0f to a lookup curve that uses normalizedSpeed

            accelerationForce = accelerationDirection * powerToTire;
        }

        //Rolling friction force
        else if (carSpeed < 0.1f && carSpeed > -0.1f) //Prevents oscilatting friction forces when speed is very low
        {
            accelerationForce = Vector3.zero;
        }
        else if (carSpeed > 0.1f)
        {
            accelerationForce = -accelerationDirection * frictionStrength;
        }
        else if (carSpeed < -0.1f)
        {
            accelerationForce = accelerationDirection * frictionStrength;
        }

        //Braking force
        if (_brakingInput > 0.0f)
        {
            float powerToTire = brakeStrength * _brakingInput;

            accelerationForce = -accelerationDirection * powerToTire;
        }
    }

    private void UpdateSteering()
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
