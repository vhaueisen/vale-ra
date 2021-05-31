using System.Collections;
using UnityEngine;

[RequireComponent(typeof(InspectionManipulator))]
public class InspectionController : ApplicationElement
{
    [SerializeField]
    private InspectionModel m_model;

    [SerializeField]
    private InspectionManipulator m_manipulator;
    private GameObject m_instance;
    private int m_current;
    public void OnInspection()
    {
        if (m_instance != MainApp.inventoryModel.ProjectionInstance)
        {
            Initialize();
        }
    }

    private void Initialize()
    {
        m_model.Bundle = MainApp.inventoryModel.ProjectionInstance.GetComponent<InspectionBundle>();
        m_model.InspectionLength = m_model.Bundle.Inspections.Length;
        m_manipulator.Initialize(m_model.Bundle);
        GenerateAnormalies();
        m_instance = MainApp.inventoryModel.ProjectionInstance;
        m_model.InspectionText.text = m_model.Bundle.Inspections[0].Name;
    }

    private void GenerateAnormalies()
    {
        m_model.AnormalieStates = new bool[m_model.InspectionLength];
        m_model.OutputStates = new bool[m_model.InspectionLength];
        for (int i = 0; i < m_model.InspectionLength; i++)
        {
            float dice = Random.Range(0f, 1f) + 1e-6f;
            m_model.AnormalieStates[i] = (dice > m_model.AnomalyThreshold);
            m_model.OutputStates[i] = false;
            if (m_model.AnormalieStates[i])
                m_manipulator.SetAnomaly(m_model.Bundle.Inspections[i], m_model.Bundle);
        }
    }

    public bool Navigate(float i, out string result)
    {
        if (Mathf.Abs(i) > 0.3f)
        {
            if (m_current + 1 < m_model.InspectionLength)
            {
                m_current++;
                result = m_model.Bundle.Inspections[m_current].Name;
                return true;
            }
        }
        result = m_model.Bundle.Inspections[m_current].Name;
        return false;
    }

    public void Hint()
    {
        StartCoroutine(IEHint(m_current));
    }

    private IEnumerator IEHint(int i)
    {
        m_manipulator.Hint(m_model.Bundle.Inspections[i]);
        yield return new WaitForSeconds(5f);
        m_manipulator.Hint(m_model.Bundle.Inspections[i], true);
        yield return new WaitForSeconds(5f);
        if (m_model.HintToggle.isOn)
            Hint();
        yield break;
    }
}
