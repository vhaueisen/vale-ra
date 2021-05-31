using UnityEngine;
using UnityEngine.UI;

public class InspectionModel : ApplicationElement
{
    public GameObject InspectionPrefab;
    public float AnomalyThreshold;
    public Text InspectionText;
    public int InspectionLength;
    public InspectionBundle Bundle;
    public InspectionView View;
    public bool[] AnormalieStates;
    public bool[] OutputStates;
    public InspectionController Controller;
    public float AnimationTime;
    public Toggle HintToggle;
}