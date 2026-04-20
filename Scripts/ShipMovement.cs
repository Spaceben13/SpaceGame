using UnityEngine;
using System;
public class ShipMovement : MonoBehaviour
{
    public GameObject Earth;
    public GameObject Moon;
    double G = 3000000;
    private float thrust;
    private float rotation;
    private float yaw;
    private double gravity_e;
    private double gravity_m;
    private double distance_earth;
    private Vector3 direction_earth;
    private double distance_moon;
    private Vector3 direction_moon;
    private bool impact;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        impact = false;
    }
    public void SetMoveInput(float thrustInput, float rotationInput, float yawInput)
    {
        thrust = thrustInput;
        rotation = rotationInput;
        yaw = yawInput;
    }

    void FixedUpdate()
    {
        distance_earth = Vector3.Distance(transform.position, Earth.transform.position);
        direction_earth = (Earth.transform.position - transform.position).normalized;

        distance_moon = Vector3.Distance(transform.position, Moon.transform.position);
        direction_moon = (Moon.transform.position - transform.position).normalized;

        gravity_e = G * 70 / Math.Pow(distance_earth, 2);

        rb.AddForce(direction_earth * (float)gravity_e);

        gravity_m = G * 10 / Math.Pow(distance_moon, 2);
        
        rb.AddForce(direction_moon * (float)gravity_m);

        // Forward/back thrust along the ship's forward axis
        rb.AddRelativeForce(Vector3.forward * thrust * 500f);

        // Strafe left/right along the ship's local X axis
        rb.AddRelativeForce(Vector3.right * yaw * -500f);

        // Rotate only on Y axis
        rb.AddRelativeTorque(Vector3.up * rotation * 2000f);
    }
    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        if (other.name == "Moon")
        {
            impact = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        GameObject other = collision.gameObject;
        if (other.name == "Moon")
        {
            impact = false;
        }
    }
}
