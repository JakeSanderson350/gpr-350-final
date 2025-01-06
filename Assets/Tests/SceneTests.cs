using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using static MovementTest;
using static TestHelpers;

public class SceneTests : InputTestFixture
{
    Mouse mouse { get => Mouse.current; }
    const string testSceneId = "Assets/Scenes/TestScene.unity";

    public override void Setup()
    {
        base.Setup();
        InputSystem.AddDevice<Keyboard>();
        InputSystem.AddDevice<Mouse>();
        SceneManager.LoadScene(testSceneId);
    }

    // Test parser expects all tests to be iterated, so we add this
    // variable to make each test run once.
    public static int[] dummyData = new int[] { 0 };

    [UnityTest]
    public IEnumerator SceneTest([ValueSource("dummyData")] int _)
    {
        yield return null;
        {
            CollisionManager collisionManager = Object.FindObjectOfType<CollisionManager>();

            Sphere s = new GameObject().AddComponent<Sphere>();
            Particle3D particle = s.gameObject.AddComponent<Particle3D>();

            s.position = new Vector3(-0.191f, -2.23f, 1.589f);
            s.Radius = .5f;

            particle.velocity = new Vector3(-1f, 0, .5f);

            AABB b = new GameObject().AddComponent<AABB>();
            b.transform.position = new Vector3(-0.74f, -2.319056f, 2.36f);

            try
            {
                Vector3 normal;
                float dist;
                CollisionDetection.collisionFns[(int)PhysicsCollider.Shape.Sphere, (int)PhysicsCollider.Shape.AABB](s, b, out normal, out dist);

                Assert.That(dist, Is.EqualTo(0.2246058f).Using(UnityEngine.TestTools.Utils.FloatEqualityComparer.Instance));
                Assert.That(normal, Is.EqualTo(new Vector3(0.17793f, 0.00000f, -0.98404f)).Using(Vec3Comparer));
            }
            catch (System.NotImplementedException)
            {
                Assert.Fail("Failed to call collision function on Sphere and AABB when looked up by shape.");
            }
        }

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

            try
            {
                Vector3 normal;
                float dist;
                CollisionDetection.collisionFns[(int)PhysicsCollider.Shape.Sphere, (int)PhysicsCollider.Shape.OBB](s, b, out normal, out dist);

                Assert.That(dist, Is.EqualTo(0.2551069f).Using(UnityEngine.TestTools.Utils.FloatEqualityComparer.Instance));
                Assert.That(normal, Is.EqualTo(new Vector3(0.94049f, 0.27164f, 0.20417f)).Using(Vec3Comparer));
            }
            catch (System.NotImplementedException)
            {
                Assert.Fail("Failed to call collision function on Sphere and AABB when looked up by shape.");
            }
        }
    }
}
