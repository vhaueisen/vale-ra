using System;
using ARInspection;

public class InspectionEventArgs : EventArgs
{
    public enum EventType
    {
        Highlight,
        Unhighlight,
    }

    public InspectionEventArgs(EventType e, Item item, Action onSucces = null)
    {
        m_type = e;
        m_onSucces = onSucces;
        m_item = item;
    }

    private EventType m_type;
    public EventType Type
    {
        get => m_type;
        set => m_type = value;
    }

    private Action m_onSucces;
    public Action OnSuccess
    {
        get => m_onSucces;
        set => m_onSucces = value;
    }
    private Item m_item;
    public Item item
    {
        get => m_item;
        set => m_item = value;
    }
}