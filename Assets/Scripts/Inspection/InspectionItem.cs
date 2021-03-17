using UnityEngine;
using UnityEngine.UI;

public class InspectionItem : MonoBehaviour
{
    public Text title;

    public void Initialize(string _title)
    {
        title.text = _title;
    }
}