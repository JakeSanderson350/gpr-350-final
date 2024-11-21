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
                if (tire.transform.rotation.y > -45)
                {
                    tire.transform.Rotate(0, -tireTurnSpeed, 0);
                }
            }
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            foreach (Tire tire in frontTires)
            {
                if (tire.transform.rotation.y < 45)
                {
                    tire.transform.Rotate(0, tireTurnSpeed, 0);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        foreach (Tire tire in frontTires)
        {
            if(tire.transform.forward != transform.forward)
            {
                tireTurnSpeed = 1.0f;
            }

            tire.UpdateForces(accelerationInput, transform.forward);

            //Debug.DrawLine(tire.transform.position, tire.transform.forward * 0.5f);

            carRB.AddForce(tire.GetForces(), tire.transform.position);
        }
        foreach (Tire tire in backTires)
        {
            tire.UpdateForces(accelerationInput, transform.forward);

            carRB.AddForce(tire.GetForces(), tire.transform.position);
        }
    }
}
