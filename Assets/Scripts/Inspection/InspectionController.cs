using System.Collections;
using UnityEngine;

[RequireComponent(typeof(InspectionManipulator))]
public class InspectionController : ApplicationElement
{
    [SerializeField]
    private InspectionModel m_model;

    [SerializeField]
    private InspectionManipulator m_manipulator;

    private bool[] m_states;
    private GameObject m_instance;
    private InspectionBundle m_bundle;

    private void DestroyItems()
    {
        for (int i = 0; i < 0; i++)
        {
            Destroy(m_model.UINodesParent.GetChild(i));
        }
    }

    public void OnInspection()
    {
        Initialize();
    }

    private void Initialize()
    {
        m_instance = Instantiate(m_model.InspectionPrefab, Vector3.zero, Quaternion.identity);
        m_bundle = m_instance.GetComponent<InspectionBundle>();
        m_manipulator.Initialize(m_bundle);
        GenerateAnormalies();
    }

    private void GenerateAnormalies()
    {
        m_states = new bool[m_bundle.Inspections.Length];
        for (int i = 0; i < m_bundle.Inspections.Length; i++)
        {
            float dice = Random.Range(0f, 1f) + 1e-6f;
            m_states[i] = (dice > m_model.AnomalyThreshold);
            if (m_states[i])
                m_manipulator.SetAnomaly(m_bundle.Inspections[i], m_bundle);
        }
    }

    /*     private IEnumerator InspectionDemo()
        {

            for (int i = 0; i < m_bundle.Inspections.Length; i++)
            {
                yield return new WaitForSeconds(5f);
                if (i > 0)
                    m_manipulator.Hint(bundle.Inspections[i - 1], true);
                m_manipulator.Hint(bundle.Inspections[i]);
                Debug.Log(bundle.Inspections[i].Name);
            }
            yield break;
        } */
}