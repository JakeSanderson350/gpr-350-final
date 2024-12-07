using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PhysicsCollider;

public class TireCollider : PhysicsCollider
{
    public Vector3 Center => transform.position;
    public float Radius = .2f;
    public Vector3 Force;

    public override Shape shape => Shape.Sphere;
}
