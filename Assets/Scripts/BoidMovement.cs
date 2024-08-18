using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidMovement : MonoBehaviour
{
    // Serialized list of all boids, used to calculate interactions between them.
    [SerializeField] private ListBoidVariable boids;

    // Radius within which other boids are considered "neighbors".
    private float radius = 2f;

    // Vision angle for the boid; defines the field of view.
    private float visionAngle = 270f;

    // Speed at which the boid moves forward.
    private float forwardSpeed = 5.0f;

    // Speed at which the boid turns.
    private float turnSpeed = 8f;

    // The current velocity of the boid, with a public getter for access.
    public Vector3 Velocity { get; private set; }

    // FixedUpdate is called at a fixed interval and is used for physics-related calculations.
    private void FixedUpdate()
    {
        // Calculate the new velocity using a combination of forward movement and boid behaviors.
        Velocity = Vector2.Lerp(Velocity, CalculateVelocity(), turnSpeed / 2f * Time.fixedDeltaTime);

        // Update the boid's position based on the calculated velocity.
        transform.position += Velocity * Time.fixedDeltaTime;

        // Rotate the boid to face the direction of its velocity.
        LookRotation();
    }

    // Calculates the new velocity based on the three boid behaviors: Separation, Alignment, and Cohesion.
    private Vector2 CalculateVelocity()
    {
        float separationBuffer = 1.7f; // Weight for the Separation behavior.
        float aligmentBuffer = 0.1f;   // Weight for the Alignment behavior.
        float cohesionBuffer = 1f;     // Weight for the Cohesion behavior.

        // Find all nearby boids within range.
        var boidInRange = BoidsInRange();

        // Calculate the combined velocity based on forward direction, separation, alignment, and cohesion.
        return ((Vector2)transform.forward
            + Separation(boidInRange) * separationBuffer
            + Alignment(boidInRange) * aligmentBuffer
            + Cohesion(boidInRange) * cohesionBuffer
                ).normalized * forwardSpeed;
    }

    // Rotates the boid smoothly towards its velocity direction.
    private void LookRotation()
    {
        transform.localRotation = Quaternion.Slerp(transform.localRotation,
            Quaternion.LookRotation(Velocity), turnSpeed * Time.fixedDeltaTime);
    }

    // Finds and returns a list of boids within the boid's range and vision cone.
    private List<BoidMovement> BoidsInRange()
    {
        var listBoid = boids.boidMovements.FindAll(boid => boid != this
            && (boid.transform.position - transform.position).magnitude <= radius
            && InVisionCone(boid.transform.position)
        );

        return listBoid;
    }

    // Checks if a given position is within the boid's vision cone.
    private bool InVisionCone(Vector2 position)
    {
        Vector2 directionToPosition = position - (Vector2)transform.position;
        float dotProduct = Vector2.Dot(transform.forward, directionToPosition);
        float cosHalfVisionAngle = Mathf.Cos(visionAngle * 0.5f * Mathf.Deg2Rad);
        return dotProduct >= cosHalfVisionAngle;
    }

    // Draws gizmos in the editor to visualize the boid's range and neighboring boids.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        // Draw a sphere around the boid to represent its interaction range.
        Gizmos.DrawWireSphere(transform.position, radius);

        var boidsInRange = BoidsInRange();

        foreach (var boid in boidsInRange)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, boid.transform.position);
        }
    }

    // Separation behavior: Keeps boids apart to avoid collisions.
    private Vector2 Separation(List<BoidMovement> boidMovements)
    {
        Vector2 direction = Vector2.zero;
        foreach (var boid in boidMovements)
        {
            // Calculate the inverse of the distance to ensure closer boids contribute more to separation.
            float ratio = Mathf.Clamp01((boid.transform.position - transform.position).magnitude / radius);
            direction -= ratio * (Vector2)(boid.transform.position - transform.position);
        }

        // Return the direction vector normalized to unit length.
        return direction.normalized;
    }

    // Alignment behavior: Aligns the boid's direction with the average direction of nearby boids.
    private Vector2 Alignment(List<BoidMovement> boidMovements)
    {
        Vector2 direction = Vector2.zero;
        foreach (var boid in boidMovements)
        {
            // Add the velocity of each neighboring boid to calculate the average direction.
            direction += (Vector2)boid.Velocity;
        }

        // Calculate the average direction if there are neighbors, otherwise continue with the current direction.
        if (boidMovements.Count > 0) direction /= boidMovements.Count;
        else direction = Velocity;

        // Return the direction vector normalized to unit length.
        return direction.normalized;
    }

    // Cohesion behavior: Steers the boid towards the average position of nearby boids.
    private Vector2 Cohesion(List<BoidMovement> boidMovements)
    {
        Vector2 direction;
        Vector2 center = Vector2.zero;

        // Calculate the center of mass of the neighboring boids.
        foreach (var boid in boidMovements) center += (Vector2)boid.transform.position;

        // Calculate the average position if there are neighbors, otherwise continue with the current position.
        if (boidMovements.Count > 0) center /= boidMovements.Count;
        else center = transform.position;

        // Calculate the direction towards the center of mass.
        direction = center - (Vector2)transform.position;

        // Return the direction vector normalized to unit length.
        return direction.normalized;
    }
}
