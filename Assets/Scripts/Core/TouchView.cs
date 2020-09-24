using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchEventArgs : EventArgs
{
    public TouchEventArgs(byte currentState_)
    {
        currentState = currentState_;
    }
    public byte currentState;
}

public class TouchView : RawTouch
{
    private byte previousState = 0;
    public event EventHandler<TouchEventArgs> TouchStateMachine;

    protected virtual void OnStateChange()
    {
        if (TouchStateMachine != null)
            TouchStateMachine(this,
                new TouchEventArgs(MainApp.touchModel.CurrentState));
    }

    // Main loop
    void LateUpdate()
    {
        if (!EventSystem.current.IsPointerOverGameObject()
            && EventSystem.current.currentSelectedGameObject == null)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            DigestMouse();
#else
            DigestTouches();
#endif
        }
        else
            MainApp.touchModel.CurrentState = TouchModel.Idle;

        if (MainApp.touchModel.CurrentState != previousState)
        {
            OnStateChange();
        }
        previousState = MainApp.touchModel.CurrentState;
    }

    // Touch position calculation and flags
    private void DigestTouches()
    {
        if (Input.touchCount == 0)
        {
            MainApp.touchModel.CurrentState = TouchModel.Idle;
        }
        else if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            MainApp.touchModel.TouchPosition = touch.position;
            MainApp.touchModel.SwipeAmount = touch.deltaPosition;

            if (touch.deltaTime >= 0.2f)
            {
                MainApp.touchModel.CurrentState = TouchModel.LongPressing;
            }
            else if (previousState != TouchModel.LongPressing)
            {
                MainApp.touchModel.CurrentState = TouchModel.Swiping;
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch[] touches = new Touch[2];
            for (int i = 0; i < 2; i++)
                touches[i] = Input.GetTouch(i);

            bool fingerMovement = false;

            foreach (Touch t in touches)
                fingerMovement = fingerMovement || (t.phase == TouchPhase.Moved);

            if (fingerMovement)
            {
                Calculate();
                MainApp.touchModel.PinchAmount = pinchDistanceDelta;
                if (previousState != TouchModel.Pinching)
                {
                    if (Mathf.Abs(pinchDistanceDelta) > 25.0f)
                    {
                        MainApp.touchModel.CurrentState = TouchModel.Pinching;
                    }
                    else
                    {
                        MainApp.touchModel.CurrentState = TouchModel.Elevating;
                        MainApp.touchModel.SwipeAmount = new Vector2(0.0f, (
                            touches[0].deltaPosition.y +
                            touches[1].deltaPosition.y) / 2.0f);
                    }
                }
                else
                {
                    if (Mathf.Abs(pinchDistanceDelta) > 1.0f)
                    {
                        MainApp.touchModel.CurrentState = TouchModel.Pinching;
                    }
                    else
                    {
                        MainApp.touchModel.CurrentState = TouchModel.Elevating;
                        MainApp.touchModel.SwipeAmount = new Vector2(0.0f, (
                            touches[0].deltaPosition.y +
                            touches[1].deltaPosition.y) / 2.0f);
                    }
                }
            }
        }
    }

    private void DigestMouse()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            MainApp.touchModel.TouchPosition = Input.mousePosition;
            MainApp.touchModel.SwipeAmount = 100.0f * (
                Input.GetAxis("Mouse X") * Vector2.right +
                Input.GetAxis("Mouse Y") * Vector2.up);
        }

        if (Input.GetMouseButton(0))
        {
            if (Input.GetKey(KeyCode.Space))
            {
                MainApp.touchModel.CurrentState = TouchModel.Pinching;
                float multiplier = 10.0f;
                if (MainApp.touchModel.SwipeAmount.x >
                    MainApp.touchModel.SwipeAmount.y)
                    multiplier = multiplier * -1.0f;
                MainApp.touchModel.PinchAmount =
                    multiplier * MainApp.touchModel.SwipeAmount.magnitude;
            }
            else
                MainApp.touchModel.CurrentState = TouchModel.Swiping;
        }
        else if (Input.GetMouseButton(1))
        {
            MainApp.touchModel.CurrentState = TouchModel.LongPressing;
        }
        else
            MainApp.touchModel.CurrentState = TouchModel.Idle;
#endif
    }
}