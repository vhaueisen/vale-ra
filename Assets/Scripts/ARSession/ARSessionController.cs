#pragma warning disable 0219

using System.Collections;
using UnityEngine.XR.ARFoundation;

public class ARSessionController : ApplicationElement
{
    private static bool m_enabled = false;
    public static bool IsSessionEnabled
    {
        get => m_enabled;
    }

    IEnumerator Start()
    {
        if ((ARSession.state == ARSessionState.None) ||
            (ARSession.state == ARSessionState.CheckingAvailability))
            yield return ARSession.CheckAvailability();

        if (ARSession.state == ARSessionState.Unsupported)
            MainApp.footerView.UnsupportedAR();
        else
            m_enabled = true;

        UpdateCollabState();
    }

    private void UpdateCollabState()
    {
        bool collab = false;
#if UNITY_IOS
        collab = m_enabled;
#endif
#if UNITY_EDITOR
        collab = true;
#endif
        MainApp.footerModel.ButtonArray[3].gameObject.SetActive(collab);
    }
}