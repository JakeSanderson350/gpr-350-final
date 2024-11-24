using log4net.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class RectRigidBody : MonoBehaviour
{ 
    //Constants
    public float mass = 1.0f;
    private float invMass;
    public Vector3 dimensions;
    public Vector3 gravity = Vector3.zero;
    private Matrix3x3 inertiaTensor;
    private Matrix3x3 inverseTensor;
    private const int TURN_SPEED = 20;

    //State variables
    public RBCenterOfMass COM; //Center of mass. Stores linear momentum

    public Vector3 angularMomentum;
    private Vector3 angularVelocity;

    public float angularDamping = 1.0f;
    private Matrix3x3 rotation;

    //Calculated variables
    private Vector3 accForces;
    private Vector3 torque;

    private void Start()
    {
        COM.velocity = Vector3.zero;
        COM.acceleration = Vector3.zero;

        angularMomentum = Vector3.zero;

        dimensions = transform.lossyScale;
        Debug.Log(dimensions);
        //dimensions = Vector3.one;
        invMass = 1.0f / mass;
        COM.inverseMass = invMass;

        inertiaTensor = calcInertiaTensor(dimensions);
        inverseTensor = inertiaTensor.Inverse();
        Debug.Log(inertiaTensor.ToStringValues());
        Debug.Log(inverseTensor.ToStringValues());

        rotation = Matrix3x3.Identity();

        accForces = Vector3.zero;
        torque = Vector3.zero;
    }

    private Matrix3x3 calcInertiaTensor(Vector3 _dimensions)
    {
        //Make diagonal matrix of inertia values for each axis
        Matrix3x3 tensor = Matrix3x3.Identity();
        float volume = _dimensions.x * _dimensions.y * _dimensions.z;

        tensor[0, 0] = mass * (_dimensions.y * _dimensions.y + _dimensions.z * _dimensions.z) / 12.0f;
        tensor[1, 1] = mass * (_dimensions.x * _dimensions.x + _dimensions.z * _dimensions.z) / 12.0f;
        tensor[2, 2] = mass * (_dimensions.x * _dimensions.x + _dimensions.y * _dimensions.y) / 12.0f;

        return tensor;
    }

    private void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;
        accForces += gravity;

        //Apply forces
        COM.AddForce(accForces);

        //Update linear momentum
        Integrator.Integrate(COM, deltaTime);
        transform.position = COM.transform.position;

        //Update angular momentum
        UpdateRotation(deltaTime);

        //reset forces
        accForces = Vector3.zero;
        torque = Vector3.zero;
    }

    private void UpdateRotation(float deltaTime)
    {
        //https://www.youtube.com/watch?v=4r_EvmPKOvY
        //https://www.cs.cmu.edu/~baraff/pbm/rigid1.pdf

        angularMomentum += torque;
        angularMomentum *= angularDamping;

        //Calculate angular velocity based on angularMomentum, rotation, and inertiaTensor
        angularVelocity = rotation * inverseTensor * rotation.Transpose() * angularMomentum;

        //Update rotation matrix
        rotation += Matrix3x3.CrossMatrix(angularVelocity) * rotation * deltaTime;
        
        //Change rotation of game object
        transform.rotation = rotation.Quaternion();

        //Get new rotation matrix based off of normalized rotation quaternion to avoid
        //accumulated floating point errors that break the rotation matrix
        rotation = Matrix3x3.QuaternionToMatrix(transform.rotation.normalized);
    }

    public void AddForce(Vector3 _newForce, Vector3 _applicationPoint)
    {
        accForces += _newForce;

        // Calculate torque produced by the force applied at the application point
        //TODO check if in bounds of rigidbody
        Vector3 pointRelativeToCenter = _applicationPoint - COM.transform.position;
        Vector3 newTorque = Vector3.Cross(_newForce, pointRelativeToCenter);
        torque += newTorque;
    }

    public Vector3 GetVelocityAtPoint(Vector3 _point)
    {
        //https://physics.stackexchange.com/questions/829797/how-to-calculate-the-velocity-of-a-point-on-a-rigid-body-that-is-both-translatin

        //Vector from center to point
        Vector3 r = _point - COM.transform.position;

        //Vector3 angularVelocity = rotation * inverseTensor * rotation.Transpose() * angularMomentum;
        Vector3 angularVelocity = Vector3.zero;
        //Calculate how rotation affects velocity
        Vector3 rotationVelocity = Vector3.Cross(angularVelocity, r);

        //Add linear and rotational velocities
        return COM.velocity + rotationVelocity;
    }
}