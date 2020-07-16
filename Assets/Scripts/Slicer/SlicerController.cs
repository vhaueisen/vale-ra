using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlicerController : ApplicationElement
{
    private SlicerModel model;
    private void Start()
    {
        model = FindObjectOfType<SlicerModel>();
    }

    private void FindReferences()
    {
        model.projectionModel = FindObjectOfType<ProjectionModel>();
        model.rotateComponent = model.projectionModel.RotateComponent;
    }

    public void Reload()
    {
        FindReferences();
        if (model.projectionModel.CurrentInstance == null)
            return;

        if (model.fresnel != null)
            Destroy(model.fresnel);

        model.materialList = GetMaterials(model.rotateComponent);
        InstantiateFresnel();
        ApplyShader(model.crossShader);
        StopAllCoroutines();

        Transform modelTransform = model.rotateComponent.GetChild(0).transform;
        BoxCollider bc = model.rotateComponent.GetChild(0).GetComponent<BoxCollider>();
        model.center = modelTransform.TransformPoint(bc.center);

        model.normals = new Vector3[] {
            modelTransform.up,
            modelTransform.right,
            modelTransform.forward
        };

        for (int i = 0; i < 3; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(model.center + model.normals[i] * 500, -model.normals[i], out hit, 750.0f, 1 << 10))
                model.lerpingPoints[i] = hit.point;
        }
        StartCoroutine(LoadShaders());
    }

    private IEnumerator LoadShaders()
    {
        yield return new WaitForEndOfFrame();
        UpadateSlice();
        yield break;
    }

    private void PositionPlanes()
    {
        List<Renderer> renderList = new List<Renderer>(model.rotateComponent.GetComponentsInChildren<Renderer>());
        Bounds boundingBox = new Bounds(model.rotateComponent.transform.position, Vector3.zero);
        foreach (Renderer r in renderList)
            boundingBox.Encapsulate(r.bounds);

        model.boxSize = boundingBox.size;
        model.offset = boundingBox.center - model.rotateComponent.transform.position;
    }

    private Material[] GetMaterials(Transform parent)
    {
        return parent.GetComponentsInChildren<Renderer>().SelectMany(o => o.materials).ToArray();
    }

    private void ApplyShader(Shader s)
    {
        foreach (Material m in model.materialList)
            m.shader = s;
    }

    private void InstantiateFresnel()
    {
        Material[] fresnelMaterials = { model.Fresnel };
        model.fresnel = Instantiate(model.rotateComponent.GetChild(0).gameObject,
            model.rotateComponent.GetChild(0).transform.position,
            model.rotateComponent.GetChild(0).transform.rotation,
            model.rotateComponent);

        Renderer[] renderList = model.fresnel.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderList)
        {
            r.material = model.Fresnel;
            r.materials = fresnelMaterials;
        }
    }

    public void ToggleHologram()
    {
        if (model.fresnel != null)
            Destroy(model.fresnel);
        else
            InstantiateFresnel();
        model.holoSlider.value = model.holoSlider.value * -1;
    }

    public void UpadateSlice()
    {
        UpadateSlice(model.sliderPos);
    }

    public void UpadateSlice(float pct)
    {
        model.sliderPos = pct;
        float t = (pct * 2.0f - 1.0f) * 1.05f;
        model.sliderLabel.text = Mathf.RoundToInt(model.sliderPos * 100.0f) + "%";
        Vector3 direction = model.normals[model.planeIndex] * (model.invertedNormals ? 1 : -1);
        float distance = t * Vector3.Distance(model.center, model.lerpingPoints[model.planeIndex]);

        foreach (Material m in model.materialList)
        {
            m.SetVector("_PlanePosition", model.center + model.normals[model.planeIndex] * distance);
            m.SetVector("_PlaneNormal", direction);
            model.Fresnel.SetVector("_PlanePosition", model.center + model.normals[model.planeIndex] * distance);
            model.Fresnel.SetVector("_PlaneNormal", direction);
        }
    }
}
