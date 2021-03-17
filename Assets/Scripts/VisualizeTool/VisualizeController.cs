using UnityEngine;

public class VisualizeController : ApplicationElement
{
    private Transform m_originTransform;
    private Transform m_cameraTransform;
    private Camera m_camera;
    private float m_velocity = 0.005f;
    private byte m_touchState = 0;
    private float maxAngle = 27.0f;
    private Vector3 center = new Vector3(0f, 0.1f, 1f);

    Vector3 ClampVector(Vector3 direction, Vector3 center, float maxAngle)
    {
        float angle = Vector3.Angle(center, direction);
        if (angle > maxAngle)
        {
            direction.Normalize();
            center.Normalize();
            Vector3 rotation = (direction - center) / angle;
            m_rigidBodies[1].angularVelocity = Vector3.zero;
            m_rigidBodies[1].velocity = Vector3.zero;
            return (rotation * maxAngle) + center;
        }
        return direction;
    }

    void Start()
    {
        MainApp.toolBoxView.toolBoxEvent += OnToolBoxEvent;
        MainApp.touchView.TouchStateMachine += OnTouchStateChange;
    }

    void OnDisable()
    {
        MainApp.toolBoxView.toolBoxEvent -= OnToolBoxEvent;
        MainApp.touchView.TouchStateMachine -= OnTouchStateChange;
    }

    public void OnToolBoxEvent(object sender, ToolBoxEventArgs eventArgs)
    {
        if (eventArgs.Key == ToolBoxEventArgs.ToolKey.visualize)
            Reload();
    }

    public void OnTouchStateChange(object sender, TouchEventArgs eventArgs)
    {
        m_touchState = eventArgs.currentState;
    }

    private Rigidbody[] m_rigidBodies;
    private void Reload()
    {
        if (SceneLoaderModel.CurrentScene.sceneIndex == SceneLoaderModel.HomeScene.sceneIndex)
        {
            ProjectionModel model = FindObjectOfType<ProjectionModel>();
            m_camera = model.MainCamera;
            m_cameraTransform = model.MainCamera.transform;
            m_originTransform = model.MainCamera.transform.parent.parent;
            m_rigidBodies = m_originTransform.parent.GetComponentsInChildren<Rigidbody>();
        }
    }

    private void LateUpdate()
    {
        if (m_cameraTransform && MainApp.toolboxModel.CurrentTool == ToolBoxEventArgs.ToolKey.visualize)
        {
            m_rigidBodies[1].transform.forward = ClampVector(m_rigidBodies[1].transform.forward, center, maxAngle);
            m_originTransform.localRotation = m_rigidBodies[0].rotation;
            m_cameraTransform.localRotation = m_rigidBodies[1].rotation;
            if (SceneLoaderModel.CurrentScene.sceneIndex == SceneLoaderModel.HomeScene.sceneIndex &&
                m_touchState == TouchModel.Pinching &&
                !MainApp.inventoryModel.inventoryWindow.state)
            {
                m_camera.fieldOfView = Mathf.Clamp(m_camera.fieldOfView - MainApp.touchModel.PinchAmount * m_velocity * 200f * Time.deltaTime, 30f, 105f);
            }
        }
    }

    private void FixedUpdate()
    {
        if (m_cameraTransform && MainApp.toolboxModel.CurrentTool == ToolBoxEventArgs.ToolKey.visualize)
        {
            if (
                SceneLoaderModel.CurrentScene.sceneIndex == SceneLoaderModel.HomeScene.sceneIndex &&
                m_touchState == TouchModel.Swiping &&
                !MainApp.inventoryModel.inventoryWindow.state
            )
            {
                m_rigidBodies[0].AddTorque(
                    Vector3.up * MainApp.touchModel.SwipeAmount.x * (m_velocity / Time.fixedDeltaTime)
                );
                m_rigidBodies[1].AddTorque(
                    transform.right * MainApp.touchModel.SwipeAmount.y * m_velocity * (0.5f / Time.fixedDeltaTime)
                );
            }
        }
    }
}