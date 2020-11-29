using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class AnchorController : ApplicationElement
{
    public AnchorModel model;
    private bool hasInstance = true;
    private ProjectionController m_projectionController;
    public void Reload()
    {
        if (m_destroyed)
        {
            model.Enabled = true;
            m_projectionController = FindObjectOfType<ProjectionController>();
            model.screenCamera = m_projectionController.projectionModel.MainCamera;
            model.elevationTarget = m_projectionController.projectionModel.RotateComponent;
            GameObject instance = Instantiate(model.AnchorPrefab, Vector3.zero, Quaternion.identity, model.elevationTarget.parent);
            instance.transform.localPosition = Vector3.zero + 0.1f * Vector3.up;
            model.anchorTransform = instance.transform;
            model.elevationRenderer = instance.GetComponent<LineRenderer>();
            model.selectionRenderer = instance.GetComponent<MeshRenderer>();
            model.elevationRenderer.material.mainTextureScale = new Vector2(SceneLoaderModel.CurrentScene.sceneIndex == SceneLoaderModel.ARScene.sceneIndex ? 9f : 5.5f, 1f);
            m_destroyed = false;
            Select();
        }
    }

    public void SetScale(float scale)
    {
        if (model.anchorTransform != null)
            model.anchorTransform.localScale = new Vector3(
                2.0f / scale,
                2.0f / scale,
                2.0f / scale
            );
    }

    public void Select()
    {
        if (m_destroyed)
            return;
        model.Enabled = true;
        model.selected = true;
        model.anchorImage.gameObject.SetActive(model.selected);
        model.selectionRenderer.enabled = true;
        if (model.anchorTransform != null)
            model.anchorImage.position = model.screenCamera.WorldToScreenPoint(model.anchorTransform.position) + Vector3.right * 15.0f;
        SetScale(m_projectionController.Scale);
    }

    public void Exit()
    {
        model.Enabled = false;
        if (!m_destroyed)
        {
            model.elevationRenderer.enabled = false;
            model.selectionRenderer.enabled = false;
        }
        model.elevating = false;
        model.selected = false;
        model.anchorImage.gameObject.SetActive(false);
    }

    private bool m_destroyed = true;
    public void Destroy()
    {
        m_destroyed = true;
        Exit();
        model.elevationRenderer = null;
        model.selectionRenderer = null;
        model.anchorTransform = null;
        model.elevationTarget = null;
    }

    public void Elevate()
    {
        model.elevateStart = Time.time;
        model.elevating = true;
    }

    private void StopElevate()
    {
        model.elevationRenderer.enabled = false;
        model.elevating = false;
    }

    public void Update()
    {
        if (model.Enabled)
        {
            if (model.elevating)
            {
                model.elevationRenderer.SetPositions(new Vector3[] { model.anchorTransform.position, model.elevationTarget.position });
                if (Time.time - model.elevateStart > model.elevateTime)
                {
                    StopElevate();
                }
                else
                    model.elevationRenderer.enabled = true;
            }
            if (model.selected)
                model.anchorImage.position = model.screenCamera.WorldToScreenPoint(model.anchorTransform.GetChild(0).position);
            if (SceneLoaderModel.CurrentScene.sceneIndex == SceneLoaderModel.ARScene.sceneIndex)
                CheckForInstance();
        }
    }

    private void CheckForInstance()
    {
        if (m_projectionController.projectionModel.CurrentInstance == null && hasInstance)
        {
            hasInstance = false;
            model.selectionRenderer.gameObject.SetActive(hasInstance);
            model.anchorImage.gameObject.SetActive(hasInstance);
        }
        else if (m_projectionController.projectionModel.CurrentInstance != null && !hasInstance)
        {
            hasInstance = true;
            model.selectionRenderer.gameObject.SetActive(hasInstance);
            model.anchorImage.gameObject.SetActive(hasInstance);
        }
    }
}