public class ProjectionController : ProjectionManipulator
{
    private byte currentState = 0;
    public bool Enabled = true;
    public float Scale
    {
        get => snapedScale;
    }

    private void Start()
    {
        MainApp.touchView.TouchStateMachine += OnTouchStateChange;
        MainApp.inventoryModel.InventoryStateMachine += OnProjectionStateChange;
        Initialize();
        if (SceneLoaderModel.CurrentScene.sceneIndex == SceneLoaderModel.HomeScene.sceneIndex)
            MainApp.bundleManager.LoadObject(null);
    }

    void OnDestroy()
    {
        MainApp.touchView.TouchStateMachine -= OnTouchStateChange;
        MainApp.inventoryModel.InventoryStateMachine -= OnProjectionStateChange;
    }

    public void OnTouchStateChange(object sender, TouchEventArgs eventArgs)
    {
        currentState = eventArgs.currentState;
    }

    public void OnProjectionStateChange(object sender, InventoryEventArgs eventArgs)
    {
        if (projectionModel.CurrentInstance != null)
            Destroy(projectionModel.CurrentInstance);
        MainApp.inventoryModel.ProjectionPrefab = eventArgs.Prefab;
    }

    private void Update()
    {
        if (!Enabled || MainApp.toolboxModel.CurrentTool != ToolBoxEventArgs.ToolKey.scaleRot && MainApp.toolboxModel.CurrentTool != ToolBoxEventArgs.ToolKey.anchor || MainApp.inventoryModel.inventoryWindow.state)
            return;

        if (currentState > 0)
        {
            if (currentState == TouchModel.Pinching && MainApp.toolboxModel.CurrentTool == ToolBoxEventArgs.ToolKey.scaleRot)
                UpdateScale(MainApp.touchModel.PinchAmount, projectionModel.ModelContainer, projectionModel.ARMode);
            else if (currentState == TouchModel.Swiping && MainApp.toolboxModel.CurrentTool == ToolBoxEventArgs.ToolKey.scaleRot)
                UpdateRotation(-MainApp.touchModel.SwipeAmount.x, projectionModel.RotateComponent, projectionModel.ARMode);
            else if (currentState == TouchModel.Swiping && MainApp.toolboxModel.CurrentTool == ToolBoxEventArgs.ToolKey.anchor)
            {
                FlexibleRaycast raycast;
                if (projectionModel.ARMode)
                {
                    raycast = Raycast(MainApp.touchModel.TouchPosition, projectionModel.RayManager, projectionModel.MainCamera);
                }
                else
                    raycast = Raycast(MainApp.touchModel.TouchPosition, projectionModel.MainCamera);
                if (!(projectionModel.CurrentInstance == null))
                {
                    UpdatePosition(raycast, projectionModel.ModelContainer, projectionModel.ARMode);
                }
                else if (MainApp.inventoryModel.CurrentModel != null)
                {
                    if (!(MainApp.inventoryModel.CurrentModel.ARImage && SceneLoaderModel.CurrentScene.sceneIndex == SceneLoaderModel.ARScene.sceneIndex))
                    {
                        projectionModel.CurrentInstance = InstantiateProjection(
                            raycast, MainApp.inventoryModel.ProjectionPrefab,
                            projectionModel.RotateComponent,
                            projectionModel.ModelContainer);
                    }
                }
            }
            else if (currentState == TouchModel.Elevating && MainApp.toolboxModel.CurrentTool == ToolBoxEventArgs.ToolKey.anchor)
            {
                MainApp.anchorController.Elevate();
                Elevate(projectionModel.RotateComponent, MainApp.touchModel.SwipeAmount.y);
            }
        }
    }

    public void HomeProjection()
    {
        FlexibleRaycast raycast = new FlexibleRaycast(HomeApp.projectionModel.HomeOrigin.transform.position,
        HomeApp.projectionModel.HomeOrigin.transform.rotation);
        projectionModel.CurrentInstance = InstantiateProjection(raycast, MainApp.inventoryModel.ProjectionPrefab, projectionModel.RotateComponent, projectionModel.ModelContainer);
    }
}