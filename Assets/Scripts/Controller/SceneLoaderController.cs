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
        StopAllCoroutines();
        StartCoroutine(LoadScene(sceneLoaderEvent.Scene));
    }

    private IEnumerator LoadScene(SceneLoaderModel.GameScene scene)
    {
        float elapsedTime = 0.0f;
        if (scene.sceneIndex != SceneLoaderModel.HomeScene.sceneIndex
            && scene.sceneIndex != SceneLoaderModel.ARScene.sceneIndex)
            MainApp.sceneLoaderModel.inventoryPanel.SetActive(false);
        else
            MainApp.sceneLoaderModel.inventoryPanel.SetActive(true);

        MainApp.sceneLoaderModel.headerText.text = scene.sceneName;
        MainApp.sceneLoaderModel.loadingPanel.SetActive(true);

        AsyncOperation sceneLoaderOperation = SceneManager.LoadSceneAsync(scene.sceneIndex);
        MainApp.sceneLoaderModel.Locomotive.localPosition = MainApp.sceneLoaderModel.LocomotiveStartPosition;
        while (!sceneLoaderOperation.isDone)
        {
            float progress = Mathf.Clamp01(sceneLoaderOperation.progress / 0.9f);
            MainApp.sceneLoaderModel.progressBar.value = progress;
            MainApp.sceneLoaderModel.Locomotive.localPosition = Vector3.Lerp(
                MainApp.sceneLoaderModel.LocomotiveStartPosition,
                MainApp.sceneLoaderModel.LocomotiveTargetPosition,
                elapsedTime / 5.0f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        GC.Collect();

        MainApp.sceneLoaderModel.loadingPanel.SetActive(false);
        SceneLoaderModel.CurrentScene = scene;
        yield break;
    }
}