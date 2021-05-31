using UnityEngine;
public class InspectionView : ApplicationElement
{
    [SerializeField]
    private InspectionModel model;

    public void OnNavigation(float i)
    {
        if (model.Controller.Navigate(i, out string result))
        {
            model.InspectionText.rectTransform.LeanAlpha(0f, model.AnimationTime).setEase(LeanTweenType.easeOutCirc).setOnComplete(
                () => model.InspectionText.rectTransform.LeanAlpha(1f, model.AnimationTime).setEase(LeanTweenType.easeInCirc).setOnComplete(
                    () => model.InspectionText.text = result
                )
            );
            if (!model.HintToggle.isOn)
                model.Controller.Hint();
        }
    }
}