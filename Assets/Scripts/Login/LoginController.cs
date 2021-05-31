using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Globalization;

public class LoginController : ApplicationElement
{
    public InputField username;
    public InputField password;
    public GameObject erroPanel;
    public GameObject pnAuth;
    public AuthLoaderController authLoader;
    public GameObject loadingPanel;
    public GameObject formHider;
    public PopupComponent popup;

    private IEnumerator GetToken()
    {
        loadingPanel.SetActive(true);
        LoginController loginData = FindObjectOfType<LoginController>();
        string username = loginData.username.text;
        string password = loginData.password.text;

        Dictionary<string, string> content = new Dictionary<string, string>();
        string clientId = "00cdebc9-332c-4b85-beb4-5e295ce8bcc5";
        content.Add("client_id", clientId);
        content.Add("grant_type", "password");
        content.Add("userName", username);
        content.Add("password", password);

        UnityWebRequest www = UnityWebRequest.Post("https://dsjwtssoauth.api.valeglobal.net/v2/.auth/token", content);
        yield return www.SendWebRequest();

        if (!(www.result == UnityWebRequest.Result.ConnectionError
            || www.result == UnityWebRequest.Result.DataProcessingError
            || www.result == UnityWebRequest.Result.ProtocolError))
        {
            int responseCode = (int)www.responseCode;
            if (responseCode == 200)
            {
                string resultContent = www.downloadHandler.text;
                LoginSettings.AuthData data = JsonUtility.FromJson<LoginSettings.AuthData>(resultContent);
                data.lastAuth = DateTime.Now.ToString();
                UpdateLoginBinaries(data);
                Connect(data);
            }
            else if (responseCode == 400)
                popup.Popup("Credenciais Inválidas", "Credencial inválida ou senha expirada!");
            else
                popup.Popup("Erro", "Erro desconhecido.");
        }
        else
            popup.Popup("Erro", "Falha de conexão com a rede.");
        loadingPanel.SetActive(false);
    }

    private void Start()
    {
        if (authLoader.Login.IsLoaded)
        {
            if (!Connect(authLoader.Login.Core))
                HideLoader();
        }
        else
            HideLoader();
    }

    private void HideLoader()
    {
        formHider.SetActive(false);
        loadingPanel.SetActive(false);
    }

    private void UpdateLoginBinaries(LoginSettings.AuthData auth)
    {
        if (auth.access_token != null)
        {
            try
            {
                TextInfo textInfo = new CultureInfo("pt-BR", false).TextInfo;
                auth.fullName = textInfo.ToTitleCase(auth.fullName.ToLower());
                auth.location = textInfo.ToTitleCase(auth.location.ToLower());
                auth.jobLevel = auth.jobLevel == null ? "Contratado" : textInfo.ToTitleCase(auth.jobLevel.ToLower());
                auth.email = auth.email.ToLower();
            }
            catch
            {

            }
        }
        DataModel.DataEventArgs eventArgs = new DataModel.DataEventArgs(DataModel.DataEventArgs.UpdateEvent, auth);
        authLoader.Login.OnSettingsEvent(this, eventArgs);
    }

    public void GetLoginData()
    {
        if (username.text != "")
            if (password.text != "")
                StartCoroutine("GetToken");
            else
                popup.Popup("Erro", "Preencha a senha!");
        else
            popup.Popup("Erro", "Preencha o usuário!");
    }

    public void Disconnect()
    {
        UpdateLoginBinaries(new LoginSettings.AuthData());
    }

    private bool Connect(LoginSettings.AuthData auth)
    {
        TimeSpan timeLeft;
        if (auth.lastAuth != "")
        {
            timeLeft = DateTime.Now.Subtract(Convert.ToDateTime(auth.lastAuth));
            if ((auth.fullName != null) && (timeLeft.Days <= 7))
            {
                SceneManager.LoadScene(1);
                return true;
            }

        }
        Disconnect();
        return false;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ForgetPassword()
    {
        popup.Popup("Esqueci a Senha",
            "O usuário é a sua matrícula (geralmente inicia-se com 01).\nA senha para acessar o jogo é a mesma senha utilizada para acessar o e-Dados (contracheques, férias e demais serviço de RH), VES ou CSP.\nCaso tenha dificuldades com a senha é possível recuperá-la através do Help Desk (no ramal telefônico 4001 ou 0800-022-4001) ou do IAM (no endereço eletrônico http://iam/).\nA senha será enviada ao seu superior imediato ou ao e-mail cadastrado previamente.");
    }
}