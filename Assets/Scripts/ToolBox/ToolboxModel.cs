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
    public float openSize
    {
        get
        {
            return containerTransform.childCount > 0 ?
                75.0f + 150.0f * (containerTransform.childCount + 1) : 0.0f;
        }
    }
    public const float speed = 7.5f;
    public Image tooboxPanel;
    public Color panelColor;
    public struct Toolbox
    {
        public Toolbox(string toolbox)
        {
            Debug.Log(toolbox);
            slice = false;
            hierarchy = false;
            animation = false;
            btnStates = new bool[] { slice, hierarchy, animation };

            if (toolbox == null || toolbox.Length < 2)
                return;

            string[] tokens = toolbox.Split(';');
            foreach (string token in tokens)
            {
                try
                {
                    string[] tool = token.Split(':');
                    string name = tool[0];
                    int value = int.Parse(tool[1]);
                    switch (name)
                    {
                        case "slicer":
                            slice = value > 0;
                            break;
                        case "hierarchy":
                            hierarchy = value > 0;
                            break;
                        case "animation":
                            animation = value > 0;
                            break;
                    }
                }
                catch
                {
                    continue;
                }
            }
#if UNITY_EDITOR
            slice = true;
            hierarchy = true;
            animation = true;
#endif
            btnStates = new bool[] { slice, hierarchy, animation };
        }
        public bool slice;
        public bool hierarchy;
        public bool animation;
        public bool[] btnStates;
    }
    public GameObject sliceBtn;
    public GameObject hierarchyBtn;
    public GameObject animationBtn;
    public RectTransform moveTool;
    public Transform hiddenTools;
}