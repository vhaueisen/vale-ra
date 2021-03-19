using System;
using System.Collections.Generic;
using System.Linq;
using ARInspection;
using UnityEngine;

public class InspectionController : ApplicationElement
{
    private InspectionModel model;
    public TextAsset textXML;
    private TextAsset previousTextXML = null;
    public RectTransform parent;
    public GameObject InspectionItemPrefab;
    private InspectionChecklist check;
    public GlowingController glowController;
    private IEnumerable<string> validGroups;
    public void Awake()
    {
        InspectionItem.InspectionEvent += OnInspection;
    }

    private void OnInspection(object sender, InspectionEventArgs eventArgs)
    {
        if (eventArgs.Type == InspectionEventArgs.EventType.Highlight)
        {
            foreach (string s in validGroups)
                if (s == eventArgs.item.Grupo)
                {
                    glowController.ChangeState(GlowingController.State.Active, eventArgs.item.Grupo);
                    eventArgs.OnSuccess();
                }
        }

        if (eventArgs.Type == InspectionEventArgs.EventType.Unhighlight)
        {
            glowController.ChangeState(GlowingController.State.Disable);
            eventArgs.OnSuccess();
        }
    }
    public void OnInspection()
    {
        DestroyItems();
        check = ReadXML();
        validGroups = glowController.Initialize(check.Item.Select(o => o.Grupo).Where(g => g != ""));
        InstantiateItens();
    }

    private void DestroyItems()
    {
        if (previousTextXML != textXML)
            for (int i = 0; i < 0; i++)
            {
                Destroy(parent.GetChild(i));
            }
        previousTextXML = textXML;
    }

    private InspectionChecklist ReadXML()
    {
        return XMLReader.FromXml<InspectionChecklist>(textXML.text);
    }

    private void InstantiateItens()
    {
        foreach (Item item in check.Item)
        {
            GameObject instance = Instantiate(InspectionItemPrefab, parent);
            instance.GetComponent<InspectionItem>().Initialize(item);
        }
    }
}