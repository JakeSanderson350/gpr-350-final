using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBCenterOfMass : Particle3D
{


    public new void FixedUpdate()
    {
        DoFixedUpdate(Time.fixedDeltaTime);
    }

    public new void DoFixedUpdate(float dt)
    {
        // Apply force from each attached ForceGenerator component
        System.Array.ForEach(GetComponents<ForceGenerator3D>(), generator => { if (generator.enabled) generator.UpdateForce(this); });

        //Let integration happen in rigidbody script
        //Integrator.Integrate(this, dt);
        ClearForces();
    }
}
