#pragma warning disable 0168
#pragma warning disable 0219
#pragma warning disable 0414

using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ProjectionModel : ApplicationElement
{
    public Camera MainCamera;
    public ARRaycastManager RayManager;
    public Transform ModelContainer;
    public Transform RotateComponent;
    public bool ARMode = false;
    public GameObject CurrentInstance = null;
    public float ScaleSpeed = 0.25e-3f;
    public float TranslateSpeed = 4.0f;
    public float RotateSpeed = 2.0f;
    public float ElevateSpeed = 0.0001f;
    public readonly float MinScale = 0.1f;
    public readonly float MaxScale = 1.0f;
    public readonly float MaxElevation = 10.0f;

    public readonly float ScaleSnapProximity = 0.02f;
    public readonly float[] ScaleSnapPoints = { 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f };
}