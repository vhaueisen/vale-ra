using System.Collections.Generic;
using UnityEngine;

public class HierarchyView : MonoBehaviour
{
    public HierarchyModel model;
    private volatile bool leaning = false;

    public void SlideToNext(List<HierarchyNodeController> targets)
    {
        if (leaning)
            return;

        model.targetPage.anchoredPosition = new Vector2(model.width, 0.0f);
        Slide(targets);
    }

    public void OnNodeChange()
    {
        model.headerTitle.text = model.currentViewing.nodeName;
    }

    public void SlideToBack(List<HierarchyNodeController> targets)
    {
        if (leaning)
            return;

        model.targetPage.anchoredPosition = new Vector2(-model.width, 0.0f);
        Slide(targets, true);
    }

    private void Slide(List<HierarchyNodeController> targets, bool back = false)
    {
        leaning = true;
        foreach (HierarchyNodeController node in targets)
        {
            node.viewTransform.SetParent(model.targetPage);
        }
        model.targetPage.LeanMoveX(0.0f, model.animDuration)
            .setEase(LeanTweenType.easeInCubic);
        model.mainPage.LeanMoveX(back ? model.width : -model.width, model.animDuration)
            .setEase(LeanTweenType.easeInCubic)
            .setOnComplete(() =>
            {
                ResetPositions();
                // if (back)
                // {
                //     foreach (HierarchyNodeController node in targets)
                //     {
                //         node.viewTransform.SetParent(model.targetPage);
                //     }
                // }
            });
        model.scrollRect.content = model.targetPage;
    }

    private void ResetPositions()
    {
        model.mainPage.anchoredPosition = model.targetPageInitialPosition;
        model.targetPage.anchoredPosition = model.mainPageInitialPosition;
        RectTransform buffer = model.mainPage;
        model.mainPage = model.targetPage;
        model.targetPage = buffer;
        model.targetPage.DetachChildren();
        leaning = false;
    }

    public void ReturnToPrevious()
    {
        HierarchyNodeController parent = model.currentViewing.parent;
        if (parent == null)
        {
            if (model.mainPage.GetChild(0) != model.hierarchyNodeRoot[0].viewTransform)
                model.view.SlideToBack(model.hierarchyNodeRoot);
        }
        else
        {
            model.currentViewing = parent;
            model.view.SlideToBack(parent.childNodes);
        }
        OnNodeChange();
    }
}