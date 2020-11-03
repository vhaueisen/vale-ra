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
    public GameObject HomeOrigin;
    public readonly float ScaleSpeed = 0.5e-3f;
    public readonly float TranslateSpeed = 5.0f;
    public readonly float RotateSpeed = 2.5f;
    public readonly float ElevateSpeed = 0.0004f;
    public readonly float MaxElevation = 10.0f;
    public readonly float ScaleSnapProximity = 0.02f;
    public Quaternion instanceRotation = Quaternion.identity;
    public AnchorController anchorController;
}