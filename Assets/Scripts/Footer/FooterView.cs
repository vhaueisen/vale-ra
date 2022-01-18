using System;

public class FooterView : ApplicationElement
{
    public static event EventHandler<SceneLoaderEventArgs> SceneLoaderEvent;

    public static void OnSceneLoader(SceneLoaderModel.GameScene scene, System.Action onComplete = null, object sender = null)
    {
        if (SceneLoaderEvent != null)
            SceneLoaderEvent(sender, new SceneLoaderEventArgs(scene, onComplete));
    }

    public void LoadHomeScene()
    {
        OnSceneLoader(SceneLoaderModel.HomeScene);
    }

    public void LoadARScene()
    {
        OnSceneLoader(SceneLoaderModel.ARScene);
    }

    public void LoadQRScene()
    {
        OnSceneLoader(SceneLoaderModel.QRScene);
    }

    public void LoadProfileScene()
    {
        // OnSceneLoader(SceneLoaderModel.ProfileScene);
    }

    public void UnsupportedAR()
    {
        MainApp.footerModel.ButtonArray[2].gameObject.SetActive(false);
    }

    public void LoadSettingsScene()
    {
        OnSceneLoader(SceneLoaderModel.SettingsScene);
    }

    public void LoadQuickStartScene()
    {
        OnSceneLoader(SceneLoaderModel.QuickStartScene);
    }

    public void LoadCollaborationScene()
    {
        OnSceneLoader(SceneLoaderModel.LearningScene);
    }

}