using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField]
    private RectRigidBody carRB;

    [SerializeField]
    private List<Tire> frontTires, backTires;
    public float tireTurnSpeed = 1;
    private float tireAngle = 0.0f;

    private float accelerationInput;
    private float brakeInput;

    // Start is called before the first frame update
    void Start()
    {
        
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
        foreach (Tire tire in frontTires)
        {
            tire.UpdateForces(accelerationInput, transform.forward);

            //Debug.DrawLine(tire.transform.position, tire.transform.forward * 0.5f);
            Debug.DrawLine(tire.transform.position, tire.transform.position + (tire.transform.forward * 2.0f), Color.blue);

            carRB.AddForce(tire.GetForces(), tire.transform.position);
        }
        foreach (Tire tire in backTires)
        {
            tire.UpdateForces(accelerationInput, transform.forward);

            carRB.AddForce(tire.GetForces(), tire.transform.position);
        }
    }
}
