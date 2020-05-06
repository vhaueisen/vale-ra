using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class SlicerView : ApplicationElement
{
    private Material[] materialList;
    public Material Fresnel;
    private GameObject fresnel;
    private Transform modelContainer;
    public Shader crossShader;
    private bool state = false;
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
        if (fresnel != null)
            Destroy(fresnel);
        //PositionPlanes();
        materialList = GetMaterials(modelContainer);
        InstantiateFresnel();
        ApplyShader(crossShader);
        StopAllCoroutines();
        StartCoroutine(LoadShaders());
    }

    private Vector3 lerpingPoints;
    private Vector3 center;

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
        {
            m.shader = s;
        }
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

        BoxCollider bc = modelContainer.GetChild(0).GetComponent<BoxCollider>();
        Transform modelTransform = modelContainer.GetChild(0).transform;
        if (bc == null)
        {
            state = false;
            return;
        }
        center = modelTransform.TransformPoint(bc.center);

        Vector3 normal = new Vector3[] { modelContainer.up, modelContainer.right, modelContainer.forward }[planeIndex];
        Vector3 direction = normal * (invertedNormals ? 1 : -1);

        if (WalkInDirection(normal))
        {
            float distance = t * Vector3.Distance(center, lerpingPoints);
            foreach (Material m in materialList)
            {
                m.SetVector("_PlanePosition", center + normal * distance);
                m.SetVector("_PlaneNormal", direction);
                Fresnel.SetVector("_PlanePosition", center + normal * distance);
                Fresnel.SetVector("_PlaneNormal", direction);
            }
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

    private bool WalkInDirection(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(center + direction * 500, -direction, out hit, 725, 1 << 10))
        {
            lerpingPoints = hit.point;
            return true;
        }
        return false;
    }
}
