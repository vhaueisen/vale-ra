using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class SlicerView : ApplicationElement
{
    private Material[] materialList;
    public Material Fresnel;
    private float KeyMoveSpeed = 0.25f;
    private Vector3[] Position = { Vector3.zero, Vector3.zero };
    private GameObject fresnel;
    private Transform modelContainer;
    private Shader crossShader;
    private Shader litShader;
    private bool state = false;
    private GameObject xSectionPlane;
    private GameObject ySectionPlane;
    private readonly string[] planeNames = { "XZ", "YZ", "XY" };
    private int planeIndex = 0;
    private bool invertedNormals = false;
    public Slider normalsSlider;
    public Text sliderTitle;
    public Text sliderLabel;
    private Vector3 minOffset;
    private Vector3 maxOffset;
    private float sliderPos;

    void Start()
    {
        crossShader = Shader.Find("Shader Graphs/TexturedCrossSection");
        litShader = Shader.Find("Universal Render Pipeline/Lit/Diffuse");
    }

    public void ToggleNormal()
    {
        invertedNormals = !invertedNormals;
        normalsSlider.value = invertedNormals ? 1 : -1;
        UpdateVectors();
    }

    public void ChangePlane(int i)
    {
        sliderTitle.text = "Plano " + planeNames[i];
        planeIndex = i;
        UpdateVectors();
    }

    public void Reload()
    {
        modelContainer = FindObjectOfType<ProjectionModel>().RotateComponent;
        state = true;
        PositionPlanes();
        materialList = GetMaterials(modelContainer);
        InstantiateFresnel();
        ApplyShader(crossShader);
        StopAllCoroutines();
        StartCoroutine(LoadShaders());
    }

    private IEnumerator LoadShaders()
    {
        yield return new WaitForEndOfFrame();
        UpdateVectors();
        yield break;
    }
    private void PositionPlanes()
    {
        List<Renderer> renderList = new List<Renderer>(modelContainer.GetComponentsInChildren<Renderer>());
        Bounds boundingBox = new Bounds();
        foreach (Renderer r in renderList)
            boundingBox.Encapsulate(r.bounds);


        minOffset = boundingBox.min - modelContainer.GetChild(0).transform.position;
        maxOffset = boundingBox.max - modelContainer.GetChild(0).transform.position;
    }

    private Material[] GetMaterials(Transform parent)
    {
        return parent.GetComponentsInChildren<Renderer>().SelectMany(o => o.materials).ToArray();
    }

    private void ApplyShader(Shader s)
    {
        foreach (Material m in materialList)
        {
            m.shader = s;
        }
    }

    private void InstantiateFresnel()
    {
        Material[] fresnelMaterials = { Fresnel };
        if (fresnel != null)
        {
            Destroy(fresnel);
            fresnel = null;
        }
        fresnel = Instantiate(modelContainer.GetChild(0).gameObject, modelContainer.GetChild(0).transform.position, modelContainer.GetChild(0).transform.rotation, modelContainer);
        Renderer[] renderList = fresnel.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderList)
        {
            r.material = Fresnel;
            r.materials = fresnelMaterials;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Reload();
        }
    }

    private void UpdateVectors()
    {
        UpdateVectors(sliderPos);
    }

    public void UpdateVectors(float _sliderPos)
    {
        Debug.Log("Hello");
        sliderPos = _sliderPos;
        sliderLabel.text = Mathf.RoundToInt(sliderPos * 100.0f) + "%";
        foreach (Material m in materialList)
        {
            if (planeIndex == 0)
            {
                float offset = Mathf.Lerp(minOffset.x, maxOffset.x, sliderPos);
                m.SetVector("_PlanePosition", modelContainer.transform.position + new Vector3(0, offset, 0));
                m.SetVector("_PlaneNormal", invertedNormals ? Vector3.up : -Vector3.up);
                Fresnel.SetVector("_PlanePosition", modelContainer.transform.position + new Vector3(0, offset, 0));
                Fresnel.SetVector("_PlaneNormal", invertedNormals ? Vector3.up : -Vector3.up);
            }
            else if (planeIndex == 1)
            {
                float offset = Mathf.Lerp(minOffset.x, maxOffset.x, sliderPos);
                m.SetVector("_PlanePosition", modelContainer.transform.position + new Vector3(offset, 0, 0));
                m.SetVector("_PlaneNormal", invertedNormals ? modelContainer.right : -modelContainer.right);
                Fresnel.SetVector("_PlanePosition", modelContainer.transform.position + new Vector3(offset, 0, 0));
                Fresnel.SetVector("_PlaneNormal", invertedNormals ? modelContainer.right : -modelContainer.right);
            }
            else if (planeIndex == 2)
            {
                float offset = Mathf.Lerp(minOffset.x, maxOffset.x, sliderPos);
                m.SetVector("_PlanePosition", modelContainer.transform.position + new Vector3(0, 0, offset));
                m.SetVector("_PlaneNormal", invertedNormals ? modelContainer.forward : -modelContainer.forward);
                Fresnel.SetVector("_PlanePosition", modelContainer.transform.position + new Vector3(0, 0, offset));
                Fresnel.SetVector("_PlaneNormal", invertedNormals ? modelContainer.forward : -modelContainer.forward);
            }
        }
    }
}
