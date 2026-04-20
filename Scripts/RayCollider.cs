using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RayCollider : MonoBehaviour
{
    public GameObject earth;
    public GameObject moon;

    private float earthRadius;
    private float moonRadius;

    public float rayLength = 10000;

    private Vector3 p1;
    private Vector3 p2;
    private Vector3 p3;
    private Vector3 p4;

    void Start()
    {
        rayLength = 10000;
        earthRadius = earth.transform.lossyScale.x / 2f;
        moonRadius = moon.transform.lossyScale.x / 2f;
    }

    void Update()
    {
        Vector3 moonToEarth = (earth.transform.position - moon.transform.position).normalized;
        Vector3 right = Vector3.Cross(moonToEarth, Vector3.up).normalized;
        Vector3 left = Vector3.Cross(moonToEarth, Vector3.down).normalized;

        p1 = earth.transform.position + right * earthRadius;
        p2 = earth.transform.position + left * earthRadius;
        //-----------------------------------------------------------

        p3 = moon.transform.position + right * 700 / 2f;
        p4 = moon.transform.position + left * 700 / 2f;

        //-----------------------------------------------------------
    }

    void FixedUpdate()
    {
        Vector3 dirOne = (p3 - p1).normalized;
        Vector3 dirTwo = (p4 - p2).normalized;

        Ray rayOne = new Ray(p3, dirOne);
        Ray rayTwo = new Ray(p4, dirTwo);

        Debug.DrawRay(p3, dirOne * rayLength);
        Debug.DrawRay(p4, dirTwo * rayLength);

        if (Physics.Raycast(rayOne, out RaycastHit hit, rayLength))
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("SKIPET KOLLIDERTE");
            }
        }

        if (Physics.Raycast(rayTwo, out RaycastHit hit2, rayLength))
        {
            if (hit2.collider.CompareTag("Player"))
            {
                Debug.Log("SKIPET KOLLIDERTE");
            }
        }
    }
}
