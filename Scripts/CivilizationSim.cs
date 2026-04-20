using UnityEngine;
using System;

public class CivilizationSim : MonoBehaviour
{
    public double Kardashev;
    public double GDP;
    public double Awareness;
    public double Panic;
    public double Mobilization;
    public double Population;
    public double PopInWork;
    public double Military_Expenditure;
    public double GDP_Revenue;
    void Start()
    {
        GDP = 139000; //Billion
        Awareness = 0;
        Panic = 0;
        Mobilization = 0;

        Population = 3.04; //Billion
        PopInWork = 0.943; //Billion

        InvokeRepeating("Calculations", 0f, 360f);
    }

    void Calculations()
    {
        GDP_Revenue = PopInWork * GDP / Population * 0.03;
        GDP += GDP_Revenue + GDP_Revenue * Mobilization / 10; 

        Military_Expenditure = GDP * 0.036f * Mobilization;
        Kardashev += Military_Expenditure * 0.0002f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
