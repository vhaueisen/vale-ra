using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollViewNoDragComponent : ScrollRect
{
    public override void OnBeginDrag(PointerEventData eventData)
    {
        return;
    }
    public override void OnDrag(PointerEventData eventData)
    {
        return;
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        return;
    }
}
