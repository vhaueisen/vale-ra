using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Android;
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
        bool cameraScene = scene.sceneIndex == SceneLoaderModel.ARScene.sceneIndex || scene.sceneIndex == SceneLoaderModel.QRScene.sceneIndex;
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera) && cameraScene)
            Permission.RequestUserPermission(Permission.Camera);
#endif

        float elapsedTime = 0.0f;
        if (scene.sceneIndex != SceneLoaderModel.HomeScene.sceneIndex
            && scene.sceneIndex != SceneLoaderModel.ARScene.sceneIndex)
            MainApp.sceneLoaderModel.inventoryPanel.SetActive(false);
        else
            MainApp.sceneLoaderModel.inventoryPanel.SetActive(true);

        MainApp.sceneLoaderModel.headerText.text = scene.sceneName;
        MainApp.sceneLoaderModel.loadingPanel.SetActive(true);

        AsyncOperation sceneLoaderOperation = SceneManager.LoadSceneAsync(scene.sceneIndex);
        MainApp.sceneLoaderModel.locomotiveAnimator.Play("TrainSlide");
        while (!sceneLoaderOperation.isDone)
        {
            float progress = Mathf.Clamp01(sceneLoaderOperation.progress / 0.9f);
            MainApp.sceneLoaderModel.progressBar.value = progress;
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        GC.Collect();
        if (cameraScene)
            yield return new WaitForSeconds(0.5f);
        MainApp.sceneLoaderModel.loadingPanel.SetActive(false);
        SceneLoaderModel.CurrentScene = scene;
        yield break;
    }
}