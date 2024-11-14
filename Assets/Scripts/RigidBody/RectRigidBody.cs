using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectRigidBody : MonoBehaviour
{
    //Center of mass.
    public Particle3D mCOM; //Stores linear velocity and acceleration

    public Vector3 angularVelocity;
    public Vector3 angularAcceleration;

    public float mass = 1.0f;

    private Vector3 accForces;
    private Vector3 torque;
    private Matrix3x3 inertiaTensor;

    private Matrix3x3 rotation;

    private void Start()
    {
        
    }
}

//// Public properties
//public Vector3 velocity;
//public Vector3 angularVelocity;
//public Vector3 acceleration;
//public Vector3 angularAcceleration;
//public float mass = 1.0f;

//// Gravity
//public Vector3 gravity = new Vector3(0, -9.81f, 0);

//// Private properties
//private Vector3 force;
//private Vector3 torque;

//void Start()
//{
//    // Initialize variables
//    velocity = Vector3.zero;
//    angularVelocity = Vector3.zero;
//    force = Vector3.zero;
//    torque = Vector3.zero;
//}

//void Update()
//{
//    // Update physics calculations
//    ApplyForces(Time.deltaTime);
//    ApplyRotation(Time.deltaTime);

//    // Apply position changes
//    transform.position += velocity * Time.deltaTime;
//    transform.rotation = Quaternion.Euler(angularVelocity * Time.deltaTime) * transform.rotation;

//    // Reset forces and torques
//    force = Vector3.zero;
//    torque = Vector3.zero;
//}

//public void AddForce(Vector3 newForce, Vector3 applicationPoint)
//{
//    // Add force to the total force
//    force += newForce;

//    // Calculate torque produced by the force applied at the application point
//    Vector3 pointRelativeToCenter = applicationPoint - transform.position;
//    Vector3 newTorque = Vector3.Cross(pointRelativeToCenter, newForce);
//    torque += newTorque;
//}

//private void ApplyForces(float deltaTime)
//{
//    // Apply gravity
//    force += gravity * mass;

//    // Calculate acceleration
//    acceleration = force / mass;

//    // Update velocity
//    velocity += acceleration * deltaTime;
//}

//private void ApplyRotation(float deltaTime)
//{
//    // Calculate angular acceleration
//    angularAcceleration = torque / mass; // Simplified for demonstration purposes

//    // Update angular velocity
//    angularVelocity += angularAcceleration * deltaTime;
//}