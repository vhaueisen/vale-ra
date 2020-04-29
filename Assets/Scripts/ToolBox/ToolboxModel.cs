using System;
using UnityEngine;
using UnityEngine.UI;

public class ToolBoxEventArgs : EventArgs
{
    public ToolBoxEventArgs(byte toolKey)
    {
        ToolKey = toolKey;
    }

    public ToolBoxEventArgs(byte toolKey, byte mode)
    {
        ToolKey = toolKey;
        Mode = mode;
    }

    public byte ToolKey;
    public const byte moveKey = 0;
    public const byte animationKey = 1;
    public const byte hierarchyKey = 2;
    public const byte sliceKey = 3;
    public byte Mode;
    public const byte Update = 0;
    public const byte Deactivate = 1;
}

public class ToolboxModel : ApplicationElement
{
    public GameObject container;
    public GameObject[] containerBtnList;
    public Image[] containerImgList;
    public RectTransform containerTransform;
    public RectTransform currentToolTransform;
    public bool state = false;
    public float openSize = 150.0f;
    public const float speed = 7.5f;
    public Image tooboxPanel;
    public Color panelColor;
}