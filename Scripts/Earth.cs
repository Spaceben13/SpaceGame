using UnityEngine;

public class Earth : MonoBehaviour
{
    public float rotationSpeed;
    void Awake()
    {
        rotationSpeed = 0.8f;
    }


    void Update()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}
