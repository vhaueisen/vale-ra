using System.Collections;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class SceneLoaderController : ApplicationElement
{
    private void Start()
    {
        LeanTween.play(MainApp.sceneLoaderModel.books, MainApp.sceneLoaderModel.bookSpriteSheet).setFrameRate(30);
        MainApp.footerView.SceneLoaderEvent += OnSceneLoader;
        if (MainApp.quickStartController.IsCompleted())
            StartCoroutine(LoadScene(SceneLoaderModel.HomeScene));
        else
            StartCoroutine(LoadScene(SceneLoaderModel.QuickStartScene));
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            OnSceneLoader(this, new SceneLoaderEventArgs(SceneLoaderModel.ARScene));
    }

    private void TweenAlpha(bool reverse)
    {
        LeanTween.alpha(MainApp.sceneLoaderModel.loadingPanel, reverse ? 1.0f : 0.0f, reverse ? 0.0f : 0.3f)
            .setOnComplete
                (() =>
                    {
                        MainApp.sceneLoaderModel.loadingPanel.gameObject.SetActive(reverse);
                        if (!reverse)
                        {
                            MainApp.sceneLoaderModel.books.LeanCancel();
                        }
                    }
                );
    }

    public void OnSceneLoader(object sender, SceneLoaderEventArgs sceneLoaderEvent)
    {
        StopAllCoroutines();
        CloseWindows();
        MainApp.anchorController.Destroy();
        MainApp.sceneLoaderModel.loadingPanel.gameObject.SetActive(true);
        LeanTween.play(MainApp.sceneLoaderModel.books, MainApp.sceneLoaderModel.bookSpriteSheet).setFrameRate(30);
        TweenAlpha(true);
        StartCoroutine(LoadScene(sceneLoaderEvent.Scene));
    }

    private IEnumerator LoadScene(SceneLoaderModel.GameScene scene)
    {
        SceneLoaderModel.CurrentScene = scene;
        bool cameraScene = scene.sceneIndex == SceneLoaderModel.ARScene.sceneIndex || scene.sceneIndex == SceneLoaderModel.QRScene.sceneIndex;
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera) && cameraScene)
            Permission.RequestUserPermission(Permission.Camera);
#endif
        if (scene.sceneIndex == SceneLoaderModel.ARScene.sceneIndex)
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        else
            Screen.sleepTimeout = SleepTimeout.SystemSetting;

        MainApp.sceneLoaderModel.headerText.text = scene.sceneName;
        AsyncOperation sceneLoaderOperation = SceneManager.LoadSceneAsync(scene.sceneIndex);
        MainApp.toolBoxView.OnSceneLoad();
        MainApp.arSessionController.OnSceneLoad(scene.sceneIndex == SceneLoaderModel.ARScene.sceneIndex);
        while (!sceneLoaderOperation.isDone)
        {
            float progress = Mathf.Clamp01(sceneLoaderOperation.progress);
            MainApp.sceneLoaderModel.progressBar.value = progress;
            yield return null;
        }
        MainApp.sceneLoaderModel.progressBar.value = 1.0f;
        if (cameraScene)
            yield return new WaitForSeconds(1.0f);
        TweenAlpha(false);
        if (scene.sceneIndex == SceneLoaderModel.QuickStartScene.sceneIndex)
            MainApp.quickStartController.Initialize();
        yield break;
    }

    private void CloseWindows()
    {
        WindowComponent[] windows = FindObjectsOfType<WindowComponent>();
        foreach (WindowComponent window in windows)
            if (window.state)
                window.Exit();
    }
}