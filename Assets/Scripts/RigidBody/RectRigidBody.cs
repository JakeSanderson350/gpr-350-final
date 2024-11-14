using log4net.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class RectRigidBody : MonoBehaviour
{
    //Center of mass.
    public Particle3D mCOM; //Stores linear velocity and acceleration

    public Vector3 angularVelocity;
    public Vector3 angularAcceleration;

    public Vector3 dimensions;
    public float mass = 1.0f;
    private float invMass;

    private Vector3 accForces;
    private Vector3 torque;
    private Matrix3x3 inertiaTensor;
    private Matrix3x3 inverseTensor;

    private Matrix3x3 rotation;

    private void Start()
    {
        mCOM.velocity = Vector3.zero;
        mCOM.acceleration = Vector3.zero;
        angularVelocity = Vector3.zero;
        angularAcceleration = Vector3.zero;

        dimensions = transform.lossyScale;
        invMass = 1.0f / mass;

        inertiaTensor = calcInertiaTensor(dimensions);
        inverseTensor = inertiaTensor.Inverse();

        rotation = Matrix3x3.Identity();
    }

    private Matrix3x3 calcInertiaTensor(Vector3 _dimensions)
    {
        //Make diagonal matrix of inertia values for each axis
        Matrix3x3 tensor = Matrix3x3.Identity();
        float volume = _dimensions.x * _dimensions.y * _dimensions.z;

        tensor[0, 0] = volume * (_dimensions.y * _dimensions.y + _dimensions.z * _dimensions.z) / 12.0f;
        tensor[1, 1] = volume * (_dimensions.x * _dimensions.x + _dimensions.z * _dimensions.z) / 12.0f;
        tensor[2, 2] = volume * (_dimensions.x * _dimensions.x + _dimensions.y * _dimensions.y) / 12.0f;

        return tensor;
    }

    private void FixedUpdate()
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

//// Size of the box (used for moment of inertia)
//public Vector3 dimensions = new Vector3(1f, 1f, 1f);

//// Private properties
//private Vector3 force;
//private Vector3 torque;

//private float I_x, I_y, I_z;

//void Start()
//{
//    // Initialize variables
//    velocity = Vector3.zero;
//    angularVelocity = Vector3.zero;
//    force = Vector3.zero;
//    torque = Vector3.zero;

//    // Calculate the moments of inertia for the box (about its center of mass)
//    I_x = (1f / 12f) * mass * (Mathf.Pow(dimensions.y, 2) + Mathf.Pow(dimensions.z, 2));
//    I_y = (1f / 12f) * mass * (Mathf.Pow(dimensions.x, 2) + Mathf.Pow(dimensions.z, 2));
//    I_z = (1f / 12f) * mass * (Mathf.Pow(dimensions.x, 2) + Mathf.Pow(dimensions.y, 2));
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
//    angularAcceleration = torque / mass; // This is the simplified case

//    // Correct angular acceleration calculation based on inertia (simplified)
//    angularAcceleration.x /= I_x;
//    angularAcceleration.y /= I_y;
//    angularAcceleration.z /= I_z;

//    // Update angular velocity
//    angularVelocity += angularAcceleration * deltaTime;
//}