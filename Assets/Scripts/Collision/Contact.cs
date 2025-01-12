using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Contact
{
    public RectRigidBody[] rigidBodies = new RectRigidBody[2];

    public Vector3 contactNormal;
    public Vector3 contactPoint;

    public float penetration;
    public float restitution;

    public void setBodyData(RectRigidBody rb1,  RectRigidBody rb2, float _restitution)
    {
        rigidBodies[0] = rb1;
        rigidBodies[1] = rb2;
        
        restitution = _restitution;
    }
}
