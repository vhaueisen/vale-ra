using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemController : ApplicationElement, IPointerClickHandler
{
    public Text tittle;
    public Image Thumbnail;
    private ARObjectScript model;

    public void Initialize(ARObjectScript arModel)
    {
        model = arModel;
        name = model.Name;
        tittle.text = model.Name;
        Thumbnail.sprite = model.Image;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ARObjectWindowController window = FindObjectOfType<ARObjectWindowController>();
        if (window)
            window.EnterARWindow(model);
    }
}