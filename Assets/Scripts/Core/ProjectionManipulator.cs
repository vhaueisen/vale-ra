using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ProjectionManipulator : ApplicationElement
{
    private float scaleBuffer = 1.0f;
    private float snapedScale = 1.0f;
    public ProjectionModel projectionModel;
    ARSessionOrigin sessionOrigin;
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
    }

    private float Snap(float f, float[] snapPoints, float proximity)
    {
        float maxDist = float.PositiveInfinity;
        int snapIndex = 0;
        for (int i = 0; i < snapPoints.Length; i++)
        {
            float dist = Mathf.Abs(snapPoints[i] - f);
            if (float.IsPositiveInfinity(maxDist) || dist < maxDist)
            {
                snapIndex = i;
                maxDist = dist;
            }
        }
        if (maxDist <= proximity)
            return snapPoints[snapIndex];
        return f;
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
        if (Physics.Raycast(cameraRay, out hit, 3.0f))
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

        posTarget.position = raycast.position;
        GameObject instance = Instantiate(prefab, rotTarget.position, raycast.rotation);
        instance.transform.localScale = Vector3.one;

        // Shoudn't be done in runtime
        // Dirty approach to fix anchoring issues
        List<MeshRenderer> meshList = new List<MeshRenderer>(instance.GetComponentsInChildren<MeshRenderer>());
        MeshRenderer instanceMesh = instance.GetComponent<MeshRenderer>();
        if (instanceMesh != null)
            meshList.Add(instanceMesh);

        Bounds boundingBox = new Bounds(rotTarget.position, Vector3.zero);

        for (int i = 0; i < meshList.Count; i++)
            boundingBox.Encapsulate(meshList[i].bounds.max);

        float maxDim = boundingBox.size.x;

        if (boundingBox.size.y > maxDim)
            maxDim = boundingBox.size.y;
        if (boundingBox.size.z > maxDim)
            maxDim = boundingBox.size.z;

        instance.transform.SetParent(rotTarget);
        instance.transform.localScale = Vector3.one;
        SetScale(0.35f / maxDim);

        float minY = meshList[0].bounds.min.y;
        foreach (MeshRenderer mesh in meshList)
            if (mesh.bounds.min.y < minY)
                minY = mesh.bounds.min.y;

        float offset = rotTarget.transform.position.y - minY + 0.001f;
        instance.transform.position = instance.transform.position + Vector3.up * offset;
        return instance;
    }

    public void UpdatePosition(FlexibleRaycast raycast, Transform target, bool isAR)
    {
        if (!raycast.isValid)
            return;

        if (isAR)
            sessionOrigin.MakeContentAppearAt(target, raycast.position);
        else
            target.position = Vector3.Lerp(target.position, raycast.position, Time.deltaTime * projectionModel.TranslateSpeed * MainApp.coreDataModel.Settings.Core.TranslateSpeed);
    }

    private void SetScale(float scale)
    {
        snapedScale = Mathf.Clamp(scale, projectionModel.MinScale, projectionModel.MaxScale);
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

        snapedScale = Snap(Mathf.Clamp(scaleBuffer, projectionModel.MinScale, projectionModel.MaxScale),
                projectionModel.ScaleSnapPoints,
                projectionModel.ScaleSnapProximity);

        if (isAR)
        {
            sessionOrigin.transform.localScale = new Vector3(1.0f / snapedScale, 1.0f / snapedScale, 1.0f / snapedScale);
        }
        else
            target.localScale = new Vector3(snapedScale, snapedScale, snapedScale);
        FindObjectOfType<ToastNotificationComponent>().Notify(string.Format("Escala: {0}%", Mathf.RoundToInt(snapedScale * 100)));
    }


    public void UpdateRotation(float desiredRotation, Transform target, bool isAR)
    {
        target.Rotate(Vector3.up * desiredRotation * Time.deltaTime * projectionModel.RotateSpeed * MainApp.coreDataModel.Settings.Core.RotateSpeed);
    }

    private float elevationOffset = 0.0f;
    public void Elevate(Transform target, float amount)
    {
        elevationOffset = projectionModel.ElevateSpeed * amount * MainApp.coreDataModel.Settings.Core.ElevateSpeed + elevationOffset;
        elevationOffset = Mathf.Clamp(elevationOffset, 0.0f, projectionModel.MaxElevation);
        Vector3 targetPos = new Vector3(target.transform.localPosition.x,
            elevationOffset / snapedScale,
            target.transform.localPosition.z);
        target.transform.localPosition = Vector3.Lerp(target.localPosition, targetPos, Time.deltaTime * projectionModel.TranslateSpeed * MainApp.coreDataModel.Settings.Core.TranslateSpeed);
        FindObjectOfType<ToastNotificationComponent>().Notify(string.Format("Elevacão: {0:0.00}m", elevationOffset));
    }
}