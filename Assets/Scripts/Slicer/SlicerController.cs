using System.Collections;
using System.Linq;
using UnityEngine;

public class SlicerController : ApplicationElement
{
    private float m_currentPos;
    public GameObject hitPoint;
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
        ApplyShader(model.crossShader);
        StopAllCoroutines();

        Transform modelTransform = model.rotateComponent.GetChild(0).transform;
        BoxCollider bc = modelTransform.GetComponent<BoxCollider>();
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
        yield return null;
        InstantiateFresnel();
        yield return null;
        UpdateSlice();
        yield break;
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
        if (model.holoSlider.value == -1)
            return;
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

    public void UpdateSlice()
    {
        UpdateSlice(m_currentPos);
    }

    public void UpdateSlice(float pct)
    {
        m_currentPos = pct;
        model.sliderLabel.text = string.Format("{0:0.0} %", m_currentPos * 100.0f);
        pct = 1.0f - pct;
        float t = (pct * 2.0f - 1.0f) * 1.1f;
        Vector3 direction = model.normals[model.planeIndex] * (model.invertedNormals ? -1 : 1);
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
