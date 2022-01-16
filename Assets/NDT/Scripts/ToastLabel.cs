using TMPro;
using UnityEngine;

public class ToastLabel : MonoBehaviour
{
    public Camera mainCam;
    public GameObject labelPrefab;
    public GameObject label;
    private Vector3 offset = new Vector3(0, 0.05f, -0.15f);
    public string ObjName;    

    void Start(){
        label = Instantiate(labelPrefab, transform.position + offset, Quaternion.identity);
        label.GetComponentInChildren<TextMeshPro>().text = ObjName;
        label.SetActive(false);
    }

    void Update()
    {
        if(label.activeInHierarchy){
            label.transform.LookAt(mainCam.transform);
            label.transform.position = transform.position + offset;
        }
    }
}
