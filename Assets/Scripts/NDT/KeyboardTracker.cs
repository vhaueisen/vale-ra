using UnityEngine;

public class KeyboardTracker : IHandTracker
{
    Vector3 target = Vector3.zero;
    public Vector3 Target { get => target; }

    HandTrackerState state = HandTrackerState.Idle;
    public HandTrackerState State { get => state; }
    private float speed = 1f;
    private float stateDebounce = 0.05f;
    private float lastState = -3f;
    private float maxD = 5f;
    public void Initialize()
    {

    }

    public void UpdateTracker()
    {
        if (Input.GetKey(KeyCode.W))
        {
            target = Vector3.MoveTowards(Target, target + Vector3.forward * speed * Time.deltaTime, maxD);
        }
        if (Input.GetKey(KeyCode.S))
        {
            target = Vector3.MoveTowards(Target, target - Vector3.forward * speed * Time.deltaTime, maxD);
        }
        if (Input.GetKey(KeyCode.D))
        {
            target = Vector3.MoveTowards(Target, target + Vector3.right * speed * Time.deltaTime, maxD);
        }

        if (Input.GetKey(KeyCode.A))
        {
            target = Vector3.MoveTowards(Target, target + -Vector3.right * speed * Time.deltaTime, maxD);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            target = Vector3.MoveTowards(Target, target + Vector3.up * speed * Time.deltaTime, maxD);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            target = Vector3.MoveTowards(Target, target - Vector3.up * speed * Time.deltaTime, maxD);
        }
        if (Input.GetKeyDown(KeyCode.Q) && Time.time >= lastState + stateDebounce)
        {
            lastState = Time.time;
            state = HandTrackerState.Grab;
        }
        if (Input.GetKeyDown(KeyCode.E) && Time.time >= lastState + stateDebounce)
        {
            lastState = Time.time;
            state = HandTrackerState.Pinch;
        }

        if (Time.time >= lastState + stateDebounce)
        {
            state = HandTrackerState.Idle;
        }
    }
}