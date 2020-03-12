using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderController : ApplicationElement
{
    private void Start()
    {
        MainApp.footerView.SceneLoaderEvent += OnSceneLoader;
        StartCoroutine(LoadScene(SceneLoaderModel.HomeScene));
    }
    public void OnSceneLoader(object sender, SceneLoaderEventArgs sceneLoaderEvent)
    {
        StartCoroutine(LoadScene(sceneLoaderEvent.Scene));
    }

    private IEnumerator LoadScene(SceneLoaderModel.GameScene scene)
    {
        if (scene.sceneIndex != SceneLoaderModel.HomeScene.sceneIndex
            && scene.sceneIndex != SceneLoaderModel.ARScene.sceneIndex)
            MainApp.sceneLoaderModel.inventoryPanel.SetActive(false);
        else
            MainApp.sceneLoaderModel.inventoryPanel.SetActive(true);

        MainApp.sceneLoaderModel.headerText.text = scene.sceneName;
        MainApp.sceneLoaderModel.loadingPanel.SetActive(true);

        AsyncOperation sceneLoaderOperation = SceneManager.LoadSceneAsync(scene.sceneIndex);

        while (!sceneLoaderOperation.isDone)
        {
            MainApp.sceneLoaderModel.progressBar.value = Mathf.Clamp01(sceneLoaderOperation.progress / 0.9f);
            yield return null;
        }
        GC.Collect();

        MainApp.sceneLoaderModel.loadingPanel.SetActive(false);
        // if (scene.sceneIndex == SceneLoaderModel.SettingsScene.sceneIndex)
        //     SettingsApp.settingsView.SettingsEvent += MainApp.coreDataModel.Settings.OnSettingsEvent;
        yield break;
    }
}