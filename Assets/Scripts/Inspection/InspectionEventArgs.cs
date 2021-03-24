using System;

public class InspectionEventArgs : EventArgs
{
    public enum EventType
    {
        Highlight,
        Unhighlight,
    }

    public InspectionEventArgs(EventType e, Action onSucces = null)
    {
        m_type = e;
        m_onSucces = onSucces;
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
}