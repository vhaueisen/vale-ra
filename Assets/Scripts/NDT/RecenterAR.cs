using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class RecenterAR : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private ManomotionManager manomotionManager;
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARSessionOrigin origin;
    [SerializeField] private Transform content;
    void Start()
    {
        manomotionManager.SetManoMotionSmoothingValue(2f);
        manomotionManager.SetManoMotionGestureSmoothingValue(1f);
    }

    public void Center()
    {
        var result = new List<ARRaycastHit>();
        if (raycastManager.Raycast(new Vector2(Screen.width / 2.0f, Screen.height / 2.0f), result))
        {
            origin.MakeContentAppearAt(content, result[0].pose.position, result[0].pose.rotation);
        }
    }
}
