using UnityEngine;

public class VisualizeController : ApplicationElement
{
    private Transform m_target;
    private Transform m_lookAt;
    private float[] m_clampedRot = new float[] { -40.0f, 40.0f };
    private float m_horizontalVelocity = 2.0f;
    private float m_verticalVelocity = 2.0f;
    private float m_xRotation = 20.0f;
    private byte m_touchState = 0;

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
        if (eventArgs.ToolKey == ToolBoxEventArgs.visualizeKey)
            Reload();
    }

    public void OnTouchStateChange(object sender, TouchEventArgs eventArgs)
    {
        m_touchState = eventArgs.currentState;
    }

    private void Reload()
    {
        ProjectionModel model = FindObjectOfType<ProjectionModel>();
        m_lookAt = model.HomeOrigin.transform;
        m_target = model.MainCamera.transform;
    }

    private void Update()
    {
        if (
            SceneLoaderModel.CurrentScene.sceneIndex == SceneLoaderModel.HomeScene.sceneIndex &&
            m_touchState > 0 &&
            m_touchState == TouchModel.Swiping &&
            MainApp.toolboxModel.CurrentTool == ToolBoxEventArgs.visualizeKey &&
            !MainApp.inventoryModel.inventoryWindow.state
        )
        {
            m_xRotation = Mathf.Clamp(m_xRotation + MainApp.touchModel.SwipeAmount.y * m_verticalVelocity * Time.deltaTime, m_clampedRot[0], m_clampedRot[1]);
            m_target.eulerAngles = new Vector3(m_xRotation, m_target.eulerAngles.y, m_target.eulerAngles.z);
            m_target.RotateAround(m_lookAt.transform.position, Vector3.up, MainApp.touchModel.SwipeAmount.x * m_horizontalVelocity * Time.deltaTime);
        }
    }
}