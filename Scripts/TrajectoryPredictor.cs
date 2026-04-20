using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryPredictor : MonoBehaviour
{
    [Header("References")]
    public GameObject Earth;
    public GameObject MoonObject;

    [Header("Simulation")]
    [Range(100, 2000)] public int steps = 1000;
    public float timeStep = 0.1f;

    [Header("Orbit Detection")]
    [Tooltip("How close (units) the path must return to its start to count as a closed orbit")]
    public float orbitCloseThreshold = 30f;
    [Tooltip("Minimum steps before orbit closure can be detected")]
    public int minOrbitSteps = 150;

    [Header("Visuals")]
    [Range(32, 256)] public int ringSegments = 128;
    public float lineWidth = 5f;
    public Color trajectoryColor = new Color(0.2f, 0.8f, 1f, 1f);

    [Header("Collision Radii")]
    [Tooltip("Trajectory stops and gravity is clamped at this distance from Earth's center")]
    public float earthSurfaceRadius = 100f;
    [Tooltip("Trajectory stops and gravity is clamped at this distance from Moon's center")]
    public float moonSurfaceRadius = 40f;

    [Header("Visibility")]
    [Tooltip("Hide trajectory when ship speed relative to Moon is below this value")]
    public float hideRelativeSpeedThreshold = 10f;

    private LineRenderer lr;
    private Moon moonScript;
    private Rigidbody shipRb;
    private Vector3[] pointBuffer;

    // Set once per SimulateAndDraw call, used by GetAcceleration
    private Vector3 simEarthPos;
    private float simMoonRadius;
    private float simMoonAngleDeg;
    private float simMoonSpeed;

    private const double G = 3000000.0;

    void Awake()
    {
        shipRb = GetComponent<Rigidbody>();
        moonScript = MoonObject.GetComponent<Moon>();
        pointBuffer = new Vector3[steps];

        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.numCapVertices = 0;

        Shader shader = Shader.Find("Universal Render Pipeline/Unlit")
                     ?? Shader.Find("Sprites/Default")
                     ?? Shader.Find("Unlit/Color");
        if (shader != null)
            lr.material = new Material(shader);
    }

    void Update()
    {
        // Hide when ship is nearly stationary relative to the Moon
        if (IsNearlyStationaryRelativeToMoon())
        {
            lr.positionCount = 0;
            return;
        }

        SimulateAndDraw();
    }

    bool IsNearlyStationaryRelativeToMoon()
    {
        Vector3 moonVel = GetMoonVelocity();
        return (shipRb.linearVelocity - moonVel).magnitude < hideRelativeSpeedThreshold;
    }

    // Computes Moon's current world-space velocity from its circular orbit
    Vector3 GetMoonVelocity()
    {
        Vector3 moonDir = MoonObject.transform.position - Earth.transform.position;
        float radius = moonDir.magnitude;
        float angleRad = Mathf.Atan2(moonDir.z, moonDir.x);
        // orbitSpeed is degrees/sec; tangent direction is (-sin, 0, cos)
        float angularRadPerSec = moonScript.orbitSpeed * Mathf.Deg2Rad;
        return new Vector3(-Mathf.Sin(angleRad), 0f, Mathf.Cos(angleRad))
               * angularRadPerSec * radius;
    }

    void SimulateAndDraw()
    {
        Vector3 simPos = transform.position;
        Vector3 simVel = shipRb.linearVelocity;

        simEarthPos  = Earth.transform.position;
        Vector3 moonDir = MoonObject.transform.position - simEarthPos;
        simMoonRadius   = moonDir.magnitude;
        simMoonAngleDeg = Mathf.Atan2(moonDir.z, moonDir.x) * Mathf.Rad2Deg;
        simMoonSpeed    = moonScript.orbitSpeed; // degrees per second

        int pointCount = 0;
        pointBuffer[pointCount++] = simPos;
        Vector3 startPos = simPos;
        int orbitCloseStep = -1;

        for (int i = 1; i < steps; i++)
        {
            float elapsed = i * timeStep;
            RK4Step(ref simPos, ref simVel, elapsed, timeStep);
            pointBuffer[pointCount++] = simPos;

            // Stop if trajectory hits a surface
            float moonAngleRad = (simMoonAngleDeg + simMoonSpeed * elapsed) * Mathf.Deg2Rad;
            Vector3 moonPosAtStep = simEarthPos + new Vector3(
                Mathf.Cos(moonAngleRad), 0f, Mathf.Sin(moonAngleRad)) * simMoonRadius;

            if (Vector3.Distance(simPos, simEarthPos) < earthSurfaceRadius ||
                Vector3.Distance(simPos, moonPosAtStep) < moonSurfaceRadius)
                break;

            if (i >= minOrbitSteps && Vector3.Distance(simPos, startPos) < orbitCloseThreshold)
            {
                orbitCloseStep = i;
                break;
            }
        }

        if (orbitCloseStep > 0)
            DrawRing(pointCount, simEarthPos);
        else
            DrawTrajectory(pointCount);
    }

    // RK4 integrator — far more stable than Euler at large timesteps
    void RK4Step(ref Vector3 pos, ref Vector3 vel, float t, float dt)
    {
        Vector3 k1a = GetAcceleration(pos, t);
        Vector3 k1v = vel;

        Vector3 k2a = GetAcceleration(pos + k1v * (dt * 0.5f), t + dt * 0.5f);
        Vector3 k2v = vel + k1a * (dt * 0.5f);

        Vector3 k3a = GetAcceleration(pos + k2v * (dt * 0.5f), t + dt * 0.5f);
        Vector3 k3v = vel + k2a * (dt * 0.5f);

        Vector3 k4a = GetAcceleration(pos + k3v * dt, t + dt);
        Vector3 k4v = vel + k3a * dt;

        pos += (k1v + 2f * k2v + 2f * k3v + k4v) * (dt / 6f);
        vel += (k1a + 2f * k2a + 2f * k3a + k4a) * (dt / 6f);
    }

    // Gravity acceleration at a given position and simulation time
    Vector3 GetAcceleration(Vector3 pos, float elapsed)
    {
        float moonAngleRad = (simMoonAngleDeg + simMoonSpeed * elapsed) * Mathf.Deg2Rad;
        Vector3 moonSimPos = simEarthPos + new Vector3(
            Mathf.Cos(moonAngleRad), 0f, Mathf.Sin(moonAngleRad)) * simMoonRadius;

        Vector3 toEarth = simEarthPos - pos;
        float dE = Mathf.Max(toEarth.magnitude, earthSurfaceRadius);
        Vector3 gE = toEarth.normalized * (float)(G * 70.0 / ((double)dE * dE));

        Vector3 toMoon = moonSimPos - pos;
        float dM = Mathf.Max(toMoon.magnitude, moonSurfaceRadius);
        Vector3 gM = toMoon.normalized * (float)(G * 10.0 / ((double)dM * dM));

        return gE + gM;
    }

    void DrawTrajectory(int pointCount)
    {
        lr.loop = false;
        lr.positionCount = pointCount;
        Color fadeColor = trajectoryColor;
        fadeColor.a = 0f;
        lr.startColor = trajectoryColor;
        lr.endColor = fadeColor;

        for (int i = 0; i < pointCount; i++)
            lr.SetPosition(i, pointBuffer[i]);
    }

    void DrawRing(int orbitPointCount, Vector3 center)
    {
        float totalDist = 0f;
        for (int i = 0; i < orbitPointCount; i++)
            totalDist += Vector3.Distance(pointBuffer[i], center);
        float avgRadius = totalDist / orbitPointCount;

        Vector3 normal = Vector3.zero;
        for (int i = 0; i < orbitPointCount - 1; i++)
        {
            Vector3 a = pointBuffer[i] - center;
            Vector3 b = pointBuffer[i + 1] - center;
            normal += Vector3.Cross(a, b);
        }
        if (normal.sqrMagnitude < 0.0001f) normal = Vector3.up;
        normal = normal.normalized;

        Vector3 right = Vector3.Cross(normal, Vector3.up);
        if (right.sqrMagnitude < 0.0001f)
            right = Vector3.Cross(normal, Vector3.forward);
        right = right.normalized;
        Vector3 fwd = Vector3.Cross(right, normal).normalized;

        lr.loop = true;
        lr.positionCount = ringSegments;
        lr.startColor = trajectoryColor;
        lr.endColor = trajectoryColor;

        for (int i = 0; i < ringSegments; i++)
        {
            float angle = (i / (float)ringSegments) * Mathf.PI * 2f;
            lr.SetPosition(i, center + (right * Mathf.Cos(angle) + fwd * Mathf.Sin(angle)) * avgRadius);
        }
    }
}
