using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrajectoryVisualizer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public int resolution = 10;
    public float timeStep = 0.1f; 
    public Slider powerSlider; 
    public Transform gunTransform; 

    private Vector3 gravity;

    void Start()
    {
        gravity = Physics.gravity;

        powerSlider.onValueChanged.AddListener(delegate { DrawTrajectory(); });
    }

    void Update()
    {
        DrawTrajectory();
    }

    void DrawTrajectory()
    {
        lineRenderer.positionCount = resolution;
        Vector3[] points = new Vector3[resolution];

        float power = powerSlider.value;
        Vector3 fireDirection = gunTransform.forward;
        Vector3 initialPosition = gunTransform.position;
        Vector3 initialVelocity = fireDirection.normalized * power;


        for (int i = 0; i < resolution; i++)
        {
            float t = i * timeStep;
            Vector3 position = initialPosition + initialVelocity * t + 0.5f * gravity * t * t;
            points[i] = position;
        }

        lineRenderer.SetPositions(points);
    }
}
