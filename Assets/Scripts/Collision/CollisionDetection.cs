using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PhysicsCollider;
using static RectRigidBody;

public static class CollisionDetection
{
    public static int CollisionChecks;

    public struct VectorDeltas
    {
        public Vector3 s1;
        public Vector3 s2;
        public static VectorDeltas zero
        {
            get
            {
                return new VectorDeltas { s1 = Vector3.zero, s2 = Vector3.zero };
            }
        }

        public void ApplyToPosition(PhysicsCollider s1, PhysicsCollider s2)
        {
            s1.position += this.s1;
            s2.position += this.s2;
        }

        public void ApplyToVelocity(PhysicsCollider s1, PhysicsCollider s2)
        {
            s1.velocity += this.s1;
            s2.velocity += this.s2;
        }
    };

    public class CollisionInfo
    {
        public Vector3 normal = Vector3.zero;
        public Vector3 contactPoint = Vector3.zero;
        public float penetration = 0;
        public float pctToMoveS1 = 0;
        public float pctToMoveS2 = 0;
        public float separatingVelocity = 0;
        public bool IsColliding => penetration > 0;
        public bool HasInfiniteMass => pctToMoveS1 + pctToMoveS2 == 0;
    }

    public delegate void NormalAndPenCalculation(PhysicsCollider s1, PhysicsCollider s2, out Vector3 normal, out float penetration);

    public static NormalAndPenCalculation[,] collisionFns = new NormalAndPenCalculation[(int)Shape.Count, (int)Shape.Count];

    static CollisionDetection()
    {
        collisionFns = new NormalAndPenCalculation[(int)Shape.Count, (int)Shape.Count];
        for (int i = 0; i < (int)Shape.Count; i++)
        {
            for (int j = 0; j < (int)Shape.Count; j++)
            {
                collisionFns[i, j] = (PhysicsCollider _, PhysicsCollider _, out Vector3 _, out float _) => throw new NotImplementedException();
            }
        }

        collisionFns[(int)Shape.Sphere, (int)Shape.Sphere] = TestSphereSphere;
        AddCollisionFns(Shape.Sphere, Shape.Plane, TestSpherePlane);

        // TODO: Add additional collider functions here
        AddCollisionFns(Shape.Sphere, Shape.AABB, TestSphereAABB);
        AddCollisionFns(Shape.Sphere, Shape.OBB, TestSphereOBB);

        // Static colliders do nothing
        NormalAndPenCalculation nop = (PhysicsCollider _, PhysicsCollider _, out Vector3 n, out float p) => { n = Vector3.zero; p = -1; };
        AddCollisionFns(Shape.OBB, Shape.Plane, nop);
        AddCollisionFns(Shape.AABB, Shape.Plane, nop);
        AddCollisionFns(Shape.AABB, Shape.OBB, nop);
        AddCollisionFns(Shape.Plane, Shape.Plane, nop);
        AddCollisionFns(Shape.OBB, Shape.OBB, nop);
        AddCollisionFns(Shape.AABB, Shape.AABB, nop);
    }

    static void AddCollisionFns(Shape s1, Shape s2, NormalAndPenCalculation fn)
    {
        NormalAndPenCalculation backwardsFn =
            (PhysicsCollider a, PhysicsCollider b, out Vector3 c, out float d) =>
            {
                fn(b, a, out c, out d);
                c = -c;
            };

        collisionFns[(int)s1, (int)s2] = fn;
        collisionFns[(int)s2, (int)s1] = backwardsFn;
    }

    public static void TestSphereSphere(PhysicsCollider shape1, PhysicsCollider shape2, out Vector3 normal, out float penetration)
    {
        Sphere s1 = shape1 as Sphere;
        Sphere s2 = shape2 as Sphere;

        Vector3 s2ToS1 = s1.Center - s2.Center;
        float dist = s2ToS1.magnitude;
        float sumOfRadii = (s1.Radius + s2.Radius);
        penetration = sumOfRadii - dist;
        normal = dist == 0 ? Vector3.zero : (s2ToS1 / dist);
    }

    public static void TestSpherePlane(PhysicsCollider s1, PhysicsCollider s2, out Vector3 normal, out float penetration)
    {
        Sphere s = s1 as Sphere;
        PlaneCollider p = s2 as PlaneCollider;

        float offset = Vector3.Dot(s.Center, p.Normal) - p.Offset;
        float dist = Mathf.Abs(offset);
        penetration = s.Radius - dist;
        normal = offset >= 0 ? p.Normal : -p.Normal;
    }

    // TODO: YOUR CODE HERE
    // Add new functions for sphere-AABB and sphere-OBB tests.
    public static void TestSphereAABB(PhysicsCollider s1, PhysicsCollider s2, out Vector3 normal, out float penetration)
    {
        Sphere s = s1 as Sphere;
        AABB aabb = s2 as AABB;

        //Calculate normal and pen
        Vector3 closestPoint;
        aabb.getClosestPoint(s.Center, out closestPoint);
        Vector3 offset = (s.Center - closestPoint);
        float dist = offset.magnitude;
        normal = offset.normalized;
        penetration = s.Radius - dist;
    }

    public static void TestSphereOBB(PhysicsCollider s1, PhysicsCollider s2, out Vector3 normal, out float penetration)
    {
        Sphere s = s1 as Sphere;
        OBB obb = s2 as OBB;

        //Calculate normal and pen
        Vector3 closestPoint;
        obb.getClosestPoint(s.Center, out closestPoint);
        Vector3 offset = (s.Center - closestPoint);
        float dist = offset.magnitude;
        normal = offset.normalized;
        penetration = s.Radius - dist;
    }

    public static CollisionInfo GetCollisionInfo(PhysicsCollider s1, PhysicsCollider s2)
    {
        CollisionInfo info = new CollisionInfo();
        NormalAndPenCalculation calc = collisionFns[(int)s1.shape, (int)s2.shape];

        try
        {
            calc(s1, s2, out info.normal, out info.penetration);
        }
        catch (NotImplementedException e)
        {
            Debug.Log($"Tried to test collision between {s1.shape} and {s2.shape}, but no collision detection function was found.");
            throw e;
        }

        {
            float sumOfInvMasses = s1.invMass + s2.invMass;
            if (sumOfInvMasses == 0) return info; // Both masses infinite, avoid divide-by-zero error
            info.pctToMoveS1 = s1.invMass / sumOfInvMasses;
            info.pctToMoveS2 = s2.invMass / sumOfInvMasses;

            info.separatingVelocity = Vector3.Dot(s1.velocity - s2.velocity, info.normal);
        }

        return info;
    }

    public static CollisionInfo GetCollisionInfo(RectRigidBody s1, RectRigidBody s2)
    {
        CollisionInfo info = new CollisionInfo();

        try
        {
            TestOBBOBB(s1.rbOBB, s2.rbOBB, out info.normal, out info.contactPoint, out info.penetration);
        }
        catch (NotImplementedException e)
        {
            Debug.Log($"Tried to test collision between {s1.rbOBB.shape} and {s2.rbOBB.shape}, but no collision detection function was found.");
            throw e;
        }

        {
            float sumOfInvMasses = s1.getInvMass() + s2.getInvMass();
            if (sumOfInvMasses == 0) return info; // Both masses infinite, avoid divide-by-zero error
            info.pctToMoveS1 = s1.getInvMass() / sumOfInvMasses;
            info.pctToMoveS2 = s2.getInvMass() / sumOfInvMasses;

            info.separatingVelocity = Vector3.Dot(s1.getVelocity() - s2.getVelocity(), info.normal);
        }

        return info;
    }

    //Added so that separating velocity can be calculated
    //without changing actual velocity of tire
    public static CollisionInfo GetCollisionInfo(PhysicsCollider s1, PhysicsCollider s2, Vector3 tireVelocity)
    {
        CollisionInfo info = new CollisionInfo();
        NormalAndPenCalculation calc = collisionFns[(int)s1.shape, (int)s2.shape];

        try
        {
            calc(s1, s2, out info.normal, out info.penetration);
        }
        catch (NotImplementedException e)
        {
            Debug.Log($"Tried to test collision between {s1.shape} and {s2.shape}, but no collision detection function was found.");
            throw e;
        }

        {
            float sumOfInvMasses = s1.invMass + s2.invMass;
            if (sumOfInvMasses == 0) return info; // Both masses infinite, avoid divide-by-zero error
            info.pctToMoveS1 = s1.invMass / sumOfInvMasses;
            info.pctToMoveS2 = s2.invMass / sumOfInvMasses;

            info.separatingVelocity = Vector3.Dot(tireVelocity - s2.velocity, info.normal);
        }

        return info;
    }

    public static void ApplyCollisionResolution(PhysicsCollider c1, PhysicsCollider c2)
    {
        CollisionChecks++;
        CollisionInfo info = GetCollisionInfo(c1, c2);

        VectorDeltas delPos = ResolvePosition(info);
        VectorDeltas delVel = ResolveVelocity(info);

        delPos.ApplyToPosition(c1, c2);
        delVel.ApplyToVelocity(c1, c2);
    }

    public static void ApplyCollisionResolution (RectRigidBody c1, RectRigidBody c2)
    {
        CollisionChecks++;
        CollisionInfo info = GetCollisionInfo(c1, c2);

        if (!info.IsColliding) return;

        // Get relative velocity at contact point
        Vector3 relVelAtContact = (c1.getVelocity() + Vector3.Cross(c1.getAngularVelocity(), info.contactPoint - c1.getPosition())) -
                                 (c2.getVelocity() + Vector3.Cross(c2.getAngularVelocity(), info.contactPoint - c2.getPosition()));

        // Calculate separating velocity along normal
        float separatingVelocity = Vector3.Dot(relVelAtContact, info.normal);
        if (separatingVelocity > 0) return; // Already separating

        float restitution = 0.5f; // Adjust this value for bounciness
        float newSepVelocity = -separatingVelocity * restitution;
        float deltaVelocity = newSepVelocity - separatingVelocity;

        // Calculate impulse magnitude
        float totalInverseMass = c1.getInvMass() + c2.getInvMass();

        // Calculate angular components
        Vector3 r1 = info.contactPoint - c1.getPosition();
        Vector3 r2 = info.contactPoint - c2.getPosition();

        // Impulse scaling for angular motion
        Vector3 impulsePerIMass = Vector3.Cross(r1, info.normal);
        Vector3 angularEffect = Vector3.Cross(impulsePerIMass, r1);

        float angularFactor = Vector3.Dot(angularEffect, info.normal);
        float totalFactor = totalInverseMass + angularFactor;

        // Calculate final impulse
        float impulse = deltaVelocity / totalFactor;
        Vector3 impulseVector = info.normal * impulse;

        // Apply impulses
        Vector3 linearChange1 = impulseVector * c1.getInvMass();
        Vector3 linearChange2 = -impulseVector * c2.getInvMass();

        Vector3 angularChange1 = Vector3.Cross(r1, impulseVector);
        Vector3 angularChange2 = Vector3.Cross(r2, -impulseVector);

        // Update velocities
        c1.setVelocity(c1.getVelocity() + linearChange1);
        c2.setVelocity(c2.getVelocity() + linearChange2);

        c1.setAngularMomentum(c1.getAngularMomentum() + angularChange1);
        c2.setAngularMomentum(c2.getAngularMomentum() + angularChange2);

        // Resolve interpenetration
        float percent = 0.8f; // Penetration resolution percentage
        float slop = 0.01f;  // Penetration allowance
        Vector3 correction = info.normal * Mathf.Max(info.penetration - slop, 0.0f) * percent / totalInverseMass;

        c1.setPosition(c1.getPosition() + (correction * c1.getInvMass()));
        c2.setPosition(c2.getPosition() - (correction * c2.getInvMass()));
    }

    public static VectorDeltas ResolvePosition(CollisionInfo info)
    {
        if (!info.IsColliding) return VectorDeltas.zero;
        if (info.HasInfiniteMass) return VectorDeltas.zero;

        return new VectorDeltas
        {
            s1 = info.pctToMoveS1 * info.normal * info.penetration,
            s2 = info.pctToMoveS2 * -info.normal * info.penetration
        };
    }

    public static VectorDeltas ResolveVelocity(CollisionInfo info)
    {
        if (!info.IsColliding) return VectorDeltas.zero;
        if (info.HasInfiniteMass) return VectorDeltas.zero;
        float restitution = 1;

        float separatingVelocity = info.separatingVelocity;
        if (separatingVelocity >= 0) return VectorDeltas.zero;
        float newSeparatingVelocity = -separatingVelocity * restitution;
        float deltaVelocity = newSeparatingVelocity - separatingVelocity;

        return new VectorDeltas
        {
            s1 = deltaVelocity * info.pctToMoveS1 * info.normal,
            s2 = deltaVelocity * info.pctToMoveS2 * -info.normal
        };
    }

    //OBB Collision stuff
    public static void TestOBBOBB(PhysicsCollider s1, PhysicsCollider s2, out Vector3 normal, out Vector3 contactPoint, out float penetration)
    {
        OBB b1 = s1 as OBB;
        OBB b2 = s2 as OBB;

        //Vector between centers of boxes
        Vector3 centers = s2.position - s1.position;

        float minPenetration = float.MaxValue;
        int minIndex = 0;
        int bestSingleAxis = 0;

        //List of axes to test
        Vector3[] axesToTest = new Vector3[15];

        //Axes of b1
        axesToTest[0] = b1.getAxis(0);
        axesToTest[1] = b1.getAxis(1);
        axesToTest[2] = b1.getAxis(2);

        //Axes of b2
        axesToTest[3] = b2.getAxis(0);
        axesToTest[4] = b2.getAxis(1);
        axesToTest[5] = b2.getAxis(2);

        //Edges of both boxes
        axesToTest[6] = Vector3.Cross(b1.getAxis(0), b2.getAxis(0));
        axesToTest[7] = Vector3.Cross(b1.getAxis(0), b2.getAxis(1));
        axesToTest[8] = Vector3.Cross(b1.getAxis(0), b2.getAxis(2));
        axesToTest[9] = Vector3.Cross(b1.getAxis(1), b2.getAxis(0));
        axesToTest[10] = Vector3.Cross(b1.getAxis(1), b2.getAxis(1));
        axesToTest[11] = Vector3.Cross(b1.getAxis(1), b2.getAxis(2));
        axesToTest[12] = Vector3.Cross(b1.getAxis(2), b2.getAxis(0));
        axesToTest[13] = Vector3.Cross(b1.getAxis(2), b2.getAxis(1));
        axesToTest[14] = Vector3.Cross(b1.getAxis(2), b2.getAxis(2));

        //Test all axis to find minPenetration
        for (int i = 0; i < axesToTest.Length; i++)
        {
            OBB.tryAxis(b1, b2, axesToTest[i], centers, i, ref minPenetration, ref minIndex);

            //Store best axis incase parallel edge collision
            if (i == 5)
            {
                bestSingleAxis = minIndex;
            }
        }

        Debug.Log("Least axis: " + minIndex + " Penetration: " + minPenetration);
        //Debug.DrawLine(b1.position, b1.position + axesToTest[minIndex], Color.blue);

        //Find collision data
        Vector3 tmpNormal = Vector3.zero;
        Vector3 tmpContactPoint = Vector3.zero;

        //Vertex of box2 in a face of box1
        if (minIndex < 3)
        {
            OBB.vertexFaceCollision(b1, b2, centers, minIndex, out tmpNormal, out tmpContactPoint);
            Debug.DrawLine(b1.position, b1.position + tmpNormal, Color.green);
            Debug.DrawLine(b2.position, tmpContactPoint, Color.magenta);
        }
        //Vertex of box1 in face of box2
        else if (minIndex < 6)
        {
            //Swap b1 and b2 and centers vector
            OBB.vertexFaceCollision(b2, b1, (centers * -1.0f), (minIndex - 3), out tmpNormal, out tmpContactPoint);
            Debug.DrawLine(b2.position, b2.position + tmpNormal, Color.green);
            Debug.DrawLine(b1.position, tmpContactPoint, Color.magenta);
        }
        //Edge of box1 on edge of box2
        else
        {
            OBB.edgeEdgeCollision(b1, b2, centers, (minIndex - 6), bestSingleAxis, out tmpNormal, out tmpContactPoint);
            Debug.DrawLine(b1.position, b1.position + tmpNormal, Color.green);
            Debug.DrawLine(b2.position, tmpContactPoint, Color.magenta);
        }

        normal = tmpNormal;
        contactPoint = tmpContactPoint;
        penetration = minPenetration;
    }
}