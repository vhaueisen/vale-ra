using UnityEngine;

public class ProjectionController : ProjectionManipulator
{
    private byte currentState = 0;
    private void Start()
    {
        MainApp.touchView.TouchStateMachine += OnTouchStateChange;
        MainApp.inventoryModel.InventoryStateMachine += OnProjectionStateChange;
        Initialize();
        projectionModel.CurrentInstance = null;
    }

    public void OnTouchStateChange(object sender, TouchEventArgs eventArgs)
    {
        currentState = eventArgs.currentState;
#if !(UNITY_EDITOR || UNITY_EDITOR_WIN)
        if (currentState == TouchModel.LongPressing)
            Handheld.Vibrate();
#endif
    }

    public void OnProjectionStateChange(object sender, InventoryEventArgs eventArgs)
    {
        if (projectionModel.CurrentInstance != null)
            Destroy(projectionModel.CurrentInstance);
        MainApp.inventoryModel.ProjectionPrefab = eventArgs.Prefab;
    }

    private void Update()
    {
        if (currentState > 0)
        {
            if (currentState == TouchModel.Pinching)
                UpdateScale(MainApp.touchModel.PinchAmount, projectionModel.ModelContainer, projectionModel.ARMode);
            else if (currentState == TouchModel.Swiping)
                UpdateRotation(-MainApp.touchModel.SwipeAmount.x, projectionModel.RotateComponent, projectionModel.ARMode);
            else if (currentState == TouchModel.LongPressing)
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
                else
                {
                    projectionModel.CurrentInstance = InstantiateProjection(raycast, MainApp.inventoryModel.ProjectionPrefab, projectionModel.RotateComponent, projectionModel.ModelContainer);
                }
            }
            else if (currentState == TouchModel.Elevating && projectionModel.ARMode)
            {
                Elevate(projectionModel.RotateComponent, MainApp.touchModel.SwipeAmount.y);
            }
        }
    }

    public void Initiate()
    {
        FlexibleRaycast raycast;
        raycast = Raycast(new Vector2(Screen.width / 2.0f, Screen.height / 2.0f), projectionModel.MainCamera);
        projectionModel.CurrentInstance = InstantiateProjection(raycast, MainApp.inventoryModel.ProjectionPrefab, projectionModel.RotateComponent, projectionModel.ModelContainer);
    }
}
