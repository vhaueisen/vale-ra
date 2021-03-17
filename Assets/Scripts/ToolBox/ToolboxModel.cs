using System;
using UnityEngine;
using UnityEngine.UI;

public class ToolBoxEventArgs : EventArgs
{
    public ToolBoxEventArgs(ToolKey key)
    {
        Key = key;
    }
    public enum ToolKey
    {
        scaleRot,
        animation,
        hierarchy,
        slice,
        anchor,
        visualize,
        inspection
    }
    public ToolKey Key;
}

public class ToolboxModel : ApplicationElement
{
    public GameObject[] containerBtnList;
    public bool state = false;
    public struct Toolbox
    {
        public Toolbox(string toolbox)
        {
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
            animation = false;
            hierarchy = false;
            // #if UNITY_EDITOR
            //             slice = true;
            //             hierarchy = true;
            //             animation = true;
            // #endif
            btnStates = new bool[] { slice, hierarchy, animation };
        }
        public bool slice;
        public bool hierarchy;
        public bool animation;
        public bool[] btnStates;
    }

    public RectTransform moveBtn;
    public RectTransform sliceBtn;
    public RectTransform hierarchyBtn;
    public RectTransform animationBtn;
    public RectTransform anchorBtn;
    public ToolBoxEventArgs.ToolKey CurrentTool = ToolBoxEventArgs.ToolKey.anchor;
    public RectTransform btnContainer;
    public RectTransform btnPanel;
    public Mask toolboxMask;
    public RectTransform toolRectangle;
    public Slider holoToggle;
}