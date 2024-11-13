# Starting Project

Start your project by creating a **private** template from this
repository: <https://github.com/CC-GPR-350/a6>. Remember to:

1.  Make your repository **private**
2.  Add me as a collaborator
3.  Set your `UNITY_EMAIL`, `UNITY_PASSWORD`, and `UNITY_SERIAL` repository
    secrets

# Goal

Enhance the code provided to detect Sphere-AABB collisions and
Sphere-OBB collisions.

# Rules

1. No specific gameplay enhancements are required. All tests will be
   done on newly created objects.

# Game Code Enhancements

## Implement data for AABB's and OBB's

1. Add whatever additional state is required to the provided classes
2. Ensure that **new state operates relative to Unity state** -- for
   example, if a Unity object with scale=[1,1,1] has an AABB attached,
   any queries to the halfWidth of the AABB should return [0.5,0.5,0.5].

## Add Sphere-AABB Collision Detection

1.  In the `CollisionDetection` class, create and implement a new
    function to test collision between spheres and AABBs. Register
    this new function in the `CollisionDetection.collisionFns` 2D
    array.

## Add Sphere-OBB Collision Detection

1.  In the `CollisionDetection` class, create and implement a new
    function to test collision between spheres and OBBs. Register
    this new function in the `CollisionDetection.collisionFns` 2D
    array.


## Making collisions work in the scene is not necessary

1.  You DO NOT NEED to make collisions work in the scene. Tests will
    only operate on the above criteria.


# Grading

Grades will be based on the above criteria, which will be assessed
automatically using the automated tests provided with the project files.

