using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Moon : MonoBehaviour
{
    public Transform Earth;
    public float orbitSpeed = 0.5f;

    float angle;
    float radius;
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        Vector3 dir = transform.position - Earth.position;
        radius = dir.magnitude;
        angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
    }

    void FixedUpdate()
    {
        angle += orbitSpeed * Time.fixedDeltaTime;

        Vector3 offset = new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            0f,
            Mathf.Sin(angle * Mathf.Deg2Rad)
        ) * radius;

        rb.MovePosition(Earth.position + offset);
    }
}
