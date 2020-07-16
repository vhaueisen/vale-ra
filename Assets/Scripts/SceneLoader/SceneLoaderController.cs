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
        MainApp.sceneLoaderModel.formHider.gameObject.SetActive(true);
        MainApp.sceneLoaderModel.formHider.LeanAlpha(1.0f, 0.2f);
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
        MainApp.sceneLoaderModel.locomotive.LeanMoveLocalX(2500.0f, 10.0f);
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

        MainApp.sceneLoaderModel.formHider.LeanAlpha(0.0f, 0.2f).setDelay(0.1f).setOnComplete(
            () => MainApp.sceneLoaderModel.formHider.gameObject.SetActive(false)
        );
        MainApp.sceneLoaderModel.locomotive.LeanCancel();
        MainApp.sceneLoaderModel.locomotive.anchoredPosition = new Vector2(400.0f, 0.0f);
        yield break;
    }
}