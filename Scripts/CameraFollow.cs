using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform ship;      
    public Vector3 offset = new Vector3(0, 2000, 0);
    public float smoothSpeed = 0.1f;

    private float currentZoom;

    void Start()
    {
        currentZoom = offset.y;
    }

    void LateUpdate()
    {
        if (ship == null) return;

        Vector3 targetPos = ship.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }

    public void PanOutIn(float scroll)
    {
        float zoomFactor = 0.10f;
        currentZoom -= scroll * currentZoom * zoomFactor;
        currentZoom = Mathf.Clamp(currentZoom, 200f, 10000f);
        offset = new Vector3(0, currentZoom, 0);
    }
}
