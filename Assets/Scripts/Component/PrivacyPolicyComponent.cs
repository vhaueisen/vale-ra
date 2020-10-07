using UnityEngine;

public class PrivacyPolicyComponent : MonoBehaviour
{
    private const string policyURL = "https://gestaodoconhecimento.blob.core.windows.net/inovation-team-cct/ValeRA/Documentos/Vale%20RA%20-%20Politica%20de%20Privacidade.html";
    public void PrivacyPolicy()
    {
        Application.OpenURL(policyURL);
    }
}
