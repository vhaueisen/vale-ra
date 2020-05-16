using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class SlicerView : ApplicationElement
{
    private Material[] materialList;
    public Material Fresnel;
    private GameObject fresnel;
    private Transform modelContainer;
    public Shader crossShader;
    private readonly string[] planeNames = { "XZ", "YZ", "XY" };
    private int planeIndex = 0;
    private bool invertedNormals = false;
    public Slider normalsSlider;
    public Text sliderTitle;
    public Text sliderLabel;
    private Vector3 offset;
    private Vector3 boxSize;
    private float sliderPos;
    public Slider holoSlider;
    private Vector3[] lerpingPoints = new Vector3[3];
    private Vector3[] normals;
    private Vector3 center;
    private ProjectionModel projectionModel;

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
        projectionModel = FindObjectOfType<ProjectionModel>();

        if (projectionModel.CurrentInstance == null)
            return;

        modelContainer = FindObjectOfType<ProjectionModel>().RotateComponent;

        if (fresnel != null)
            Destroy(fresnel);

        materialList = GetMaterials(modelContainer);
        InstantiateFresnel();
        ApplyShader(crossShader);
        StopAllCoroutines();

        Transform modelTransform = modelContainer.GetChild(0).transform;
        BoxCollider bc = modelContainer.GetChild(0).GetComponent<BoxCollider>();
        center = modelTransform.TransformPoint(bc.center);

        normals = new Vector3[] {
            modelTransform.up,
            modelTransform.right,
            modelTransform.forward
        };

        for (int i = 0; i < 3; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(center + normals[i] * 500, -normals[i], out hit, 750.0f, 1 << 10))
                lerpingPoints[i] = hit.point;
        }
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
        Bounds boundingBox = new Bounds(modelContainer.transform.position, Vector3.zero);
        foreach (Renderer r in renderList)
            boundingBox.Encapsulate(r.bounds);

        boxSize = boundingBox.size;
        offset = boundingBox.center - modelContainer.transform.position;
    }

    private Material[] GetMaterials(Transform parent)
    {
        return parent.GetComponentsInChildren<Renderer>().SelectMany(o => o.materials).ToArray();
    }

    private void ApplyShader(Shader s)
    {
        foreach (Material m in materialList)
            m.shader = s;
    }

    private void InstantiateFresnel()
    {
        Material[] fresnelMaterials = { Fresnel };
        fresnel = Instantiate(modelContainer.GetChild(0).gameObject, modelContainer.GetChild(0).transform.position, modelContainer.GetChild(0).transform.rotation, modelContainer);
        Renderer[] renderList = fresnel.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderList)
        {
            r.material = Fresnel;
            r.materials = fresnelMaterials;
        }
    }

    private void UpdateVectors()
    {
        UpdateVectors(sliderPos);
    }

    public void UpdateVectors(float _sliderPos)
    {
        sliderPos = _sliderPos;
        float t = (_sliderPos * 2.0f - 1.0f) * 1.05f;
        sliderLabel.text = Mathf.RoundToInt(sliderPos * 100.0f) + "%";
        Vector3 direction = normals[planeIndex] * (invertedNormals ? 1 : -1);
        float distance = t * Vector3.Distance(center, lerpingPoints[planeIndex]);

        foreach (Material m in materialList)
        {
            m.SetVector("_PlanePosition", center + normals[planeIndex] * distance);
            m.SetVector("_PlaneNormal", direction);
            Fresnel.SetVector("_PlanePosition", center + normals[planeIndex] * distance);
            Fresnel.SetVector("_PlaneNormal", direction);
        }
    }

    public void ToggleHologram()
    {
        if (fresnel != null)
            Destroy(fresnel);
        else
            InstantiateFresnel();
        holoSlider.value = holoSlider.value * -1;
    }
}