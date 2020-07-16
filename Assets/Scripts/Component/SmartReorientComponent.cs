using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartReorientComponent : MonoBehaviour
{
    // Start is called before the first frame update
    private float defaultAngle;
    private float currentAngle;
    private RectTransform rect;
    void Start()
    {
        rect = this.GetComponent<RectTransform>();
        defaultAngle = rect.rotation.eulerAngles.z;

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.deviceOrientation == DeviceOrientation.Portrait)
        {
            currentAngle = defaultAngle;
        }
        else if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
        {
            currentAngle = defaultAngle - 90.0f;
        }
        else if (Input.deviceOrientation == DeviceOrientation.LandscapeRight)
        {
            currentAngle = defaultAngle + 90.0f;
        }
        else if (Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown)
        {
            currentAngle = defaultAngle + 180.0f;
        }
        rect.rotation = Quaternion.Euler(0, 0, currentAngle);
    }
}
