using UnityEngine;

public interface IHandTracker
{
    Vector3 Target { get; }
    HandTrackerState State
    {
        get;
    }
    void UpdateTracker();
    void Initialize();
}