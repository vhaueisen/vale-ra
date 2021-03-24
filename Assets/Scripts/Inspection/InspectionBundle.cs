using UnityEngine;

[System.Serializable]
public class InspectionMisfortune
{
    public enum Type
    {
        Default,
        Missing
    }
    [SerializeField]
    private string m_name;
    public string Name
    {
        get => m_name;
    }

    [SerializeField]
    private GameObject[] m_enable;
    public GameObject[] Enable
    {
        get => m_enable;
    }

    [SerializeField]
    private GameObject[] m_disable;
    public GameObject[] Disable
    {
        get => m_disable;
    }

    [SerializeField]
    private GameObject[] m_missing;
    public GameObject[] Missing
    {
        get => m_missing;
    }

    [SerializeField]
    private Type m_t;
    public Type T
    {
        get => m_t;
    }
}


[System.Serializable]
public class InspectionGroup
{
    [SerializeField]
    private string m_name;
    public string Name
    {
        get => m_name;
    }

    [SerializeField]
    private GameObject[] m_assets;
    public GameObject[] Assets
    {
        get => m_assets;
    }

    [SerializeField]
    private int m_misfortuneIdx = -1;
    public int MisfortuneIdx
    {
        get => m_misfortuneIdx;
    }

    /*     [SerializeField]
        private InspectionMisfortune m_misfortune;
        public InspectionMisfortune Misfortune
        {
            get => m_misfortune;
        } */
}

public class InspectionBundle : MonoBehaviour
{
    [SerializeField]
    private InspectionMisfortune[] m_fortunes;
    public InspectionMisfortune[] Misfortune
    {
        get => m_fortunes;
    }

    [SerializeField]
    private InspectionGroup[] m_inspections;
    public InspectionGroup[] Inspections
    {
        get => m_inspections;
    }
}
