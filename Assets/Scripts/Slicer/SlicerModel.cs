using UnityEngine;
using UnityEngine.UI;

public class SlicerModel : ApplicationElement
{
    public Material[] materialList;
    public Material Fresnel;
    public GameObject fresnel;
    public Transform rotateComponent;
    public Shader crossShader;
    public readonly string[] planeNames = { "XZ", "YZ", "XY" };
    public int planeIndex = 0;
    public bool invertedNormals = false;
    public Slider normalsSlider;
    public Text sliderTitle;
    public Text sliderLabel;
    public Vector3 offset;
    public Vector3 boxSize;
    public Slider holoSlider;
    public Vector3[] lerpingPoints = new Vector3[3];
    public Vector3[] normals;
    public Vector3 center;
    public ProjectionModel projectionModel;
    public SlicerController controller;
    public SlicerView view;
}