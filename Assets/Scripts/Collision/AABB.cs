using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABB : PhysicsCollider
{
    // TODO: YOUR CODE HERE
    Vector3 min;
    Vector3 max;

    Vector3 halfWidths;

    public void getClosestPoint(Vector3 _point, out Vector3 closestPoint)
    {
        halfWidths = transform.lossyScale * 0.5f;
        min = transform.position - halfWidths;
        max = transform.position + halfWidths;

        closestPoint = new Vector3(
            Mathf.Clamp(_point.x, min.x, max.x),
            Mathf.Clamp(_point.y, min.y, max.y),
            Mathf.Clamp(_point.z, min.z, max.z)
         );
    }

    public override Shape shape => Shape.AABB;
}
