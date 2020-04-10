
using UnityEngine;
using UnityEngine.UI;

public class ToolboxModel : ApplicationElement
{
    public GameObject container;
    public GameObject[] containerBtnList;
    public RectTransform containerTransform;
    public bool state = false;
    public float openSize = 150.0f;
    public const float speed = 7.5f;
    public Image tooboxPanel;
    public Color panelColor;
}