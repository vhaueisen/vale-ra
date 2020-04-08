using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LoginController : ApplicationElement
{
    public InputField username;
    public InputField password;
    public GameObject erroPanel;
    public GameObject pnAuth;
    public AuthLoaderController authLoader;

    private IEnumerator GetToken()
    {
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

        if (!www.isNetworkError)
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
                loginData.UpdateWarningMessage("Credencial inválida ou senha expirada!");
            else
                loginData.UpdateWarningMessage("Erro!");
        }
        else
        {
            loginData.UpdateWarningMessage("Falha de conexão com a rede.");
        }
    }

    private void Start()
    {
        if (authLoader.Login.IsLoaded)
            Connect(authLoader.Login.Core);
    }

    private void UpdateLoginBinaries(LoginSettings.AuthData auth)
    {
        DataModel.DataEventArgs eventArgs = new DataModel.DataEventArgs(DataModel.DataEventArgs.UpdateEvent, auth);
        authLoader.Login.OnSettingsEvent(this, eventArgs);
    }

    public void GetLoginData()
    {
        if (username.text != "")
            if (password.text != "")
                // if (!username.text.StartsWith("C0"))
                StartCoroutine("GetToken");
            // else
            //     UpdateWarningMessage("Contratados não podem acessar esta aplicação.");
            else
                UpdateWarningMessage("Preencha a senha!");
        else
            UpdateWarningMessage("Preencha o usuário!");
    }

    public void UpdateWarningMessage(string message)
    {
        erroPanel.GetComponent<Text>().text = message;
        StartCoroutine(DisableErroPanel(erroPanel));
    }

    IEnumerator DisableErroPanel(GameObject panel)
    {
        yield return new WaitForSeconds(4);
        erroPanel.GetComponent<Text>().text = "";
    }

    public void Disconnect()
    {
        UpdateLoginBinaries(new LoginSettings.AuthData());
    }

    private void Connect(LoginSettings.AuthData auth)
    {
        TimeSpan timeLeft;
        if (auth.lastAuth != "")
        {
            timeLeft = DateTime.Now.Subtract(Convert.ToDateTime(auth.lastAuth));
            if ((auth.fullName != null) && (timeLeft.Days <= 7))
            {
                SceneManager.LoadScene(1);
                return;
            }

        }
        Disconnect();
    }

    public void Exit()
    {
        Application.Quit();
    }
}