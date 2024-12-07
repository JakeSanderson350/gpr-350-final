using System.Collections;
using System.Collections.Generic;
using static CollisionDetection;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField]
    private RectRigidBody carRB;

    [SerializeField]
    private List<Tire> frontTires, backTires;
    public float tireTurnSpeed = 1;
    private float tireAngle = 0.0f;
    private List<Sphere> tireColliders;

    private float accelerationInput;
    private float brakeInput;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("LoadColliders", 0.25f);
    }

    private void LoadColliders()
    {
        foreach (Tire tire in frontTires)
        {
            tireColliders.Add(tire.sphereCollider);
        }
        foreach (Tire tire in backTires)
        {
            tireColliders.Add(tire.sphereCollider);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        accelerationInput = Input.GetKey(KeyCode.UpArrow) ? 1.0f : 0.0f;

        brakeInput = Input.GetKey(KeyCode.DownArrow) ? 1.0f : 0.0f;
        
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            foreach (Tire tire in frontTires)
            {
                if (tireAngle > -90)
                {
                    tire.transform.Rotate(0, -tireTurnSpeed, 0);
                    tireAngle -= tireTurnSpeed;
                }
            }
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            foreach (Tire tire in frontTires)
            {
                if (tireAngle < 90)
                {
                    tire.transform.Rotate(0, tireTurnSpeed, 0);
                    tireAngle += tireTurnSpeed;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        //Check collision
        CollisionResolution();

        //Update tire forces
        foreach (Tire tire in frontTires)
        {
            tire.UpdateForces(accelerationInput, brakeInput, transform.forward);

            carRB.AddForce(tire.GetForces(), tire.transform.position);
        }
        foreach (Tire tire in backTires)
        {
            tire.UpdateForces(accelerationInput, brakeInput, transform.forward);

            carRB.AddForce(tire.GetForces(), tire.transform.position);
        }
    }

    private void CollisionResolution()
    {
        PlaneCollider[] planes = FindObjectsOfType<PlaneCollider>();

        foreach (Sphere tire in tireColliders)
        {
            foreach (PlaneCollider plane in planes)
            {
                //Get restitution velocity of tire
                CollisionInfo info = GetCollisionInfo(tire, plane);
                VectorDeltas deltaVelelocity = ResolveVelocity(info);
                Vector3 tireDeltaVelocity = deltaVelelocity.s1;

                //Apply force to rigidbody
                Vector3 collisionForce = (1 / tire.invMass) * tireDeltaVelocity; //F = mv
                carRB.AddForce(collisionForce, tire.transform.position);
            }
        }
    }
}
