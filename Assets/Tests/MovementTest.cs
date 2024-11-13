using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using static TestHelpers;
using System.Collections.Generic;

public class MovementTest : InputTestFixture
{
    public static int[] dummyData = { 0 };

    [Test]
    public void TestSphereAABB([ValueSource("dummyData")] int _)
    {
        Sphere s = new GameObject().AddComponent<Sphere>();
        Particle3D particle = s.gameObject.AddComponent<Particle3D>();

        s.position = new Vector3(-0.191f, -2.23f, 1.589f);
        s.Radius = .5f;

        particle.velocity = new Vector3(-1f, 0, .5f);

        AABB b = new GameObject().AddComponent<AABB>();
        b.transform.position = new Vector3(-0.74f, -2.319056f, 2.36f);

        CollisionDetection.ApplyCollisionResolution(s, b);

        Assert.That(s.position, Is.EqualTo(new Vector3(-0.15104f, -2.23000f, 1.36798f)).Using(Vec3Comparer), "Sphere position incorrect");
        Assert.That(s.velocity, Is.EqualTo(new Vector3(-0.76160f, 0.00000f, -0.81852f)).Using(Vec3Comparer), "Sphere velocity incorrect");
    }

    [Test]
    public void TestSphereOBBRotated([ValueSource("dummyData")] int _)
    {
        Sphere s = new GameObject().AddComponent<Sphere>();
        Particle3D particle = s.gameObject.AddComponent<Particle3D>();

        s.position = new Vector3(-0.191f, -2.23f, 1.589f);
        s.Radius = .5f;

        particle.velocity = new Vector3(-1f, 0, .5f);

        OBB b = new GameObject().AddComponent<OBB>();
        b.transform.position = new Vector3(-0.74f, -2.319056f, 2.36f);
        b.transform.rotation = Quaternion.Euler(new Vector3(0, 20.43f, 16.11f));
        b.transform.localScale = new Vector3(1f, 1f, 1f);

        CollisionDetection.ApplyCollisionResolution(s, b);

        Assert.That(s.position, Is.EqualTo(new Vector3(-0.00199f, -2.16912f, 1.49256f)).Using(Vec3Comparer), "Sphere position incorrect");
        Assert.That(s.velocity, Is.EqualTo(new Vector3(0.84021f, 0.59273f, -0.43899f)).Using(Vec3Comparer), "Sphere velocity incorrect");
    }

    [Test]
    public void TestSphereOBBRotatedScaled([ValueSource("dummyData")] int _)
    {
        Sphere s = new GameObject().AddComponent<Sphere>();
        Particle3D particle = s.gameObject.AddComponent<Particle3D>();

        s.position = new Vector3(0.39f, -2.07f, 2.91f);
        s.Radius = .5f;

        particle.velocity = Vector3.left;

        OBB b = new GameObject().AddComponent<OBB>();
        b.transform.position = new Vector3(-0.74f, -2.319056f, 2.36f);
        b.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 16.11f));
        b.transform.localScale = new Vector3(1.83f, 1f, 1f);

        CollisionDetection.ApplyCollisionResolution(s, b);

        Assert.That(s.position, Is.EqualTo(new Vector3(0.62993f, -2.00070f, 2.96209f)).Using(Vec3Comparer), "Sphere position incorrect");
        Assert.That(s.velocity, Is.EqualTo(new Vector3(0.76905f, 0.51095f, 0.38404f)).Using(Vec3Comparer), "Sphere velocity incorrect");
    }

    
}
