using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoaderController : ApplicationElement
{
    private Graphic[] graphics;
    private float[] alphas;
    private void Start()
    {
        graphics = MainApp.sceneLoaderModel.loadingPanel.GetComponentsInChildren<Graphic>(true);
        alphas = graphics.Select(o => o.color.a).ToArray();
        MainApp.footerView.SceneLoaderEvent += OnSceneLoader;
        StartCoroutine(LoadScene(SceneLoaderModel.HomeScene));
    }

    private void TweenAlpha(bool reverse)
    {
        for (int i = 0; i < graphics.Length; i++)
            LeanTween.alpha(graphics[i].rectTransform, reverse ? alphas[i] : 0.0f, reverse ? 0.0f : 0.2f)
                .setRecursive(false)
                    .setOnComplete
                    (() =>
                        {
                            MainApp.sceneLoaderModel.loadingPanel.gameObject.SetActive(reverse);
                            if (!reverse)
                            {
                                MainApp.sceneLoaderModel.locomotive.LeanCancel();
                                MainApp.sceneLoaderModel.locomotive.anchoredPosition = new Vector2(400.0f, 0.0f);
                            }
                        }
                    );
    }

    public void OnSceneLoader(object sender, SceneLoaderEventArgs sceneLoaderEvent)
    {
        StopAllCoroutines();
        CloseWindows();
        MainApp.sceneLoaderModel.loadingPanel.gameObject.SetActive(true);
        MainApp.sceneLoaderModel.books.SetActive(true);
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
        MainApp.sceneLoaderModel.headerText.text = scene.sceneName;
        AsyncOperation sceneLoaderOperation = SceneManager.LoadSceneAsync(scene.sceneIndex);
        MainApp.sceneLoaderModel.locomotive.LeanMoveLocalX(2500.0f, 10.0f);
        while (!sceneLoaderOperation.isDone)
        {
            float progress = Mathf.Clamp01(sceneLoaderOperation.progress);
            MainApp.sceneLoaderModel.progressBar.value = progress;
            yield return null;
        }
        MainApp.sceneLoaderModel.progressBar.value = 1.0f;
        if (cameraScene)
            yield return new WaitForSeconds(1.0f);
        MainApp.sceneLoaderModel.books.SetActive(false);
        TweenAlpha(false);
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