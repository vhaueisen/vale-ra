using UnityEngine;

public class PerformanceController : ApplicationElement
{
    public Behaviour[] performantBehaviours;
    public Renderer[] performantRenderers;
    public Light[] performantLights;

    void Start()
    {
        foreach (Behaviour b in performantBehaviours)
            b.enabled = ARSessionController.IsSessionEnabled;
        foreach (Renderer r in performantRenderers)
            r.enabled = ARSessionController.IsSessionEnabled;
        foreach (Light l in performantLights)
            l.shadows = ARSessionController.IsSessionEnabled
                ? LightShadows.Soft : LightShadows.None;
    }
}