using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Interactable : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Material[] targetMaterials;
    private enum internalState
    {
        Idle, Hover, Clicked
    }
    private Material startMaterial;
    private Renderer[] rs;
    public IHandTracker tracker
    {
        get => _tracker;
        set
        {
            if (value == null && grabbed == this)
                return;
            _tracker = value;
        }
    }
    private IHandTracker _tracker;
    private internalState state = internalState.Clicked;
    private int materialsIndex = -1;
    public static Interactable grabbed = null;
    void Awake()
    {
        rs = GetComponentsInChildren<Renderer>();
        SetState(internalState.Idle);
    }

    void Update()
    {
        if (grabbed == this)
            StaticUpdate();
        else
        {
            if (tracker == null)
            {
                SetState(internalState.Idle);
            }
            else
                SetState(tracker.State == HandTrackerState.Grab ? internalState.Clicked : internalState.Hover);
        }
    }

    void StaticUpdate()
    {
        if (tracker.State == HandTrackerState.Grab && state != internalState.Clicked)
        {
            grabbed = null;
            tracker = null;
            SetState(internalState.Idle);
        }
        else
            state = tracker.State == HandTrackerState.Grab ? internalState.Clicked : internalState.Hover;
    }

    void SetState(internalState s)
    {
        if (s == state)
            return;

        if (grabbed != null)
            return;

        state = s;

        if (s == internalState.Idle)
        {
            SetMaterials(0);
        }
        if (s == internalState.Hover)
        {
            SetMaterials(1);
        }
        else if (s == internalState.Clicked)
        {
            SetMaterials(2);
            if (grabbed == null)
                grabbed = this;
        }
    }

    void SetMaterials(int i)
    {
        foreach (var r in rs)
        {
            r.sharedMaterial = targetMaterials[i];
            r.material = targetMaterials[i];
        }
    }
}
