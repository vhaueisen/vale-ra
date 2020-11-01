using System.Collections;
using UnityEngine.XR.ARFoundation;

public class ARSessionController : ApplicationElement
{
    public ARSession session;
    IEnumerator Start()
    {
        if ((ARSession.state == ARSessionState.None) ||
            (ARSession.state == ARSessionState.CheckingAvailability))
        {
            yield return ARSession.CheckAvailability();
        }

        if (ARSession.state == ARSessionState.Unsupported)
        {
            MainApp.footerView.UnsupportedAR();
            session.enabled = false;
        }
    }

    public void OnSceneLoad(bool arScene)
    {
        session.enabled = arScene;
    }
}