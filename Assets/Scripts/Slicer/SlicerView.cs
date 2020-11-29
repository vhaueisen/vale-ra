public class SlicerView : ApplicationElement
{
    private SlicerModel model;
    private void Start()
    {
        model = FindObjectOfType<SlicerModel>();
        model.sliderTitle.text = "Plano " + model.planeNames[0];
    }

    public void ToggleNormal()
    {
        model.invertedNormals = !model.invertedNormals;
        model.normalsSlider.value = model.invertedNormals ? -1 : 1;
        model.controller.UpdateSlice();
    }

    public void ChangePlane(int i)
    {
        model.sliderTitle.text = "Plano " + model.planeNames[i];
        model.planeIndex = i;
        model.controller.UpdateSlice();
    }

    public void UpdateVectors(float _sliderPos)
    {
        model.controller.UpdateSlice(_sliderPos);
    }

    public void ToggleHologram()
    {
        model.controller.ToggleHologram();
    }
}