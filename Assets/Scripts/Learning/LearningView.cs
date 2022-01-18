using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearningView : MonoBehaviour
{
    public GameObject AccordionPrefab;
    public RectTransform ScrollContent;

    void Start()
    {
        foreach (var t in Trainer.TrainingArray)
        {
            var i = GameObject.Instantiate(AccordionPrefab, Vector3.zero, Quaternion.identity, ScrollContent);
            i.GetComponent<TrainingAccordion>().Initialize(t);
        }
    }

}
