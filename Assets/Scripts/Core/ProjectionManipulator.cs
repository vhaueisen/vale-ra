using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ProjectionManipulator : ApplicationElement
{
    private float scaleBuffer = 1.0f;
    public float snapedScale = 1.0f;
    public ProjectionModel projectionModel;
    private ARSessionOrigin sessionOrigin;
    private ARAnchor anchor;
    private ARAnchorManager anchorManager;
    public struct FlexibleRaycast
    {
        public FlexibleRaycast(Pose _pose, RaycastHit _hit, bool _isValid, bool _isAR)
        {
            pose = _pose;
            isValid = _isValid;
            isAR = _isAR;
            hit = _hit;
            if (isAR)
            {
                position = _pose.position;
                rotation = _pose.rotation;
            }
            else
            {
                position = _hit.point;
                rotation = Quaternion.identity;
            }
        }

        public FlexibleRaycast(Vector3 _position, Quaternion _rotation)
        {
            pose = new Pose();
            hit = new RaycastHit();
            position = _position;
            rotation = _rotation;
            isValid = true;
            isAR = false;
        }

        public Pose pose;
        public RaycastHit hit;
        public Vector3 position;
        public Quaternion rotation;
        public bool isValid;
        public bool isAR;
    }

    public void Initialize()
    {
        projectionModel = FindObjectOfType<ProjectionModel>();
        sessionOrigin = FindObjectOfType<ARSessionOrigin>();
        anchorManager = FindObjectOfType<ARAnchorManager>();
    }

    // AR Overload
    public FlexibleRaycast Raycast(Vector2 point, ARRaycastManager manager, Camera c)
    {
        List<ARRaycastHit> hit = new List<ARRaycastHit>();
        manager.Raycast(point, hit, TrackableType.PlaneWithinPolygon);
        if (hit.Count > 0)
        {
            return new FlexibleRaycast(hit[0].pose, new RaycastHit(), true, true);
        }

        else
            return new FlexibleRaycast(Pose.identity, new RaycastHit(), false, true);
    }

    // non AR overload
    public FlexibleRaycast Raycast(Vector2 point, Camera c)
    {
        Ray cameraRay = c.ScreenPointToRay(point);
        RaycastHit hit;
        if (Physics.Raycast(cameraRay, out hit, 30.0f, 1 << 8))
        {
            if (hit.transform.name == "BackPlane")
            {
                return new FlexibleRaycast(Pose.identity, hit, true, false);
            }
        }
        return new FlexibleRaycast(Pose.identity, new RaycastHit(), false, false);
    }

    public GameObject InstantiateProjection(FlexibleRaycast raycast, GameObject prefab, Transform rotTarget, Transform posTarget)
    {
        if (!raycast.isValid || prefab == null)
            return null;

        if (m_elevateTarget != null)
            Elevate(m_elevateTarget, -1e8f, false);

        SetScale(1.0f);
        posTarget.position = raycast.position;
        GameObject instance = Instantiate(prefab, rotTarget.position, raycast.rotation);
        projectionModel.instanceRotation = raycast.rotation;
        instance.transform.position = instance.transform.position + Vector3.down * MainApp.inventoryModel.CurrentModel.YOffset;
        instance.transform.SetParent(rotTarget);
        instance.transform.localScale = Vector3.one;
        SetScale(MainApp.inventoryModel.CurrentModel.InitialScaleFactor);
        ReAnchor(raycast.pose);
        return instance;
    }

    public void UpdatePosition(FlexibleRaycast raycast, Transform target, bool isAR)
    {
        if (!raycast.isValid)
            return;

        if (isAR)
        {
            ReAnchor(raycast.pose);
            sessionOrigin.MakeContentAppearAt(target, raycast.position);
        }
        else
            target.position = Vector3.Lerp(target.position, raycast.position, Time.deltaTime * projectionModel.TranslateSpeed * MainApp.coreDataModel.Settings.Core.TranslateSpeed);
    }

    private void ReAnchor(Pose pose)
    {
        if (anchorManager != null)
        {
            if (anchor != null)
                anchorManager.RemoveAnchor(anchor);
            anchor = anchorManager.AddAnchor(pose);
        }
    }

    private void SetScale(float scale)
    {
        snapedScale = Mathf.Clamp(scale, MainApp.inventoryModel.CurrentModel.MinScaleFactor, MainApp.inventoryModel.CurrentModel.MaxScaleFactor);
        scaleBuffer = snapedScale;

        if (projectionModel.ARMode)
        {
            sessionOrigin.transform.localScale = new Vector3(1.0f / snapedScale, 1.0f / snapedScale, 1.0f / snapedScale);
        }
        else
            projectionModel.ModelContainer.localScale = new Vector3(snapedScale, snapedScale, snapedScale);
    }

    public void UpdateScale(float pinchAmount, Transform target, bool isAR)
    {
        pinchAmount = pinchAmount * projectionModel.ScaleSpeed * MainApp.coreDataModel.Settings.Core.ScaleSpeed;

        scaleBuffer = scaleBuffer * (pinchAmount + 1);

        snapedScale = Snap(Mathf.Clamp(scaleBuffer, MainApp.inventoryModel.CurrentModel.MinScaleFactor, MainApp.inventoryModel.CurrentModel.MaxScaleFactor),
                projectionModel.ScaleSnapProximity);

        if (isAR)
        {
            sessionOrigin.transform.localScale = new Vector3(1.0f / snapedScale, 1.0f / snapedScale, 1.0f / snapedScale);
        }
        else
            target.localScale = new Vector3(snapedScale, snapedScale, snapedScale);
        MainApp.notificationComponent.Notify(string.Format("Escala: {0}%", Mathf.RoundToInt(snapedScale * 100)));

        if (m_elevateTarget != null)
            Elevate(m_elevateTarget, 0.0f, false);
    }

    private float Snap(float f, float proximity)
    {
        float dist = f % 0.1f;
        if (dist <= proximity)
            return (f - dist);
        return f;
    }

    public void UpdateRotation(float desiredRotation, Transform target, bool isAR)
    {
        if (isAR)
            projectionModel.HomeOrigin.transform.Rotate(-Vector3.up * desiredRotation * Time.deltaTime * projectionModel.RotateSpeed * MainApp.coreDataModel.Settings.Core.RotateSpeed);
        else
            target.Rotate(Vector3.up * desiredRotation * Time.deltaTime * projectionModel.RotateSpeed * MainApp.coreDataModel.Settings.Core.RotateSpeed);
    }

    private float elevationOffset = 0.0f;
    private Transform m_elevateTarget;

    public void Elevate(Transform target, float amount, bool notificate = true)
    {
        m_elevateTarget = target;
        elevationOffset = projectionModel.ElevateSpeed * amount * MainApp.coreDataModel.Settings.Core.ElevateSpeed + elevationOffset;
        elevationOffset = Mathf.Clamp(elevationOffset, 0.0f, projectionModel.MaxElevation);
        Vector3 targetPos = new Vector3(target.transform.localPosition.x,
            elevationOffset / snapedScale,
            target.transform.localPosition.z);
        target.transform.localPosition = Vector3.Lerp(target.localPosition, targetPos, Time.deltaTime * projectionModel.TranslateSpeed * MainApp.coreDataModel.Settings.Core.TranslateSpeed);
        if (notificate)
            MainApp.notificationComponent.Notify(string.Format("Elevacão: {0:0.00}m", elevationOffset));
    }
}