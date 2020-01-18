using System;

public class SceneLoaderEventArgs : EventArgs
{
    public SceneLoaderEventArgs(SceneLoaderModel.GameScene scene)
    {
        Scene = scene;
    }
    public SceneLoaderModel.GameScene Scene;
}

public class FooterView : ApplicationElement
{
    public event EventHandler<SceneLoaderEventArgs> SceneLoaderEvent;

    protected virtual void OnSceneLoader(SceneLoaderModel.GameScene scene)
    {
        if (SceneLoaderEvent != null)
            SceneLoaderEvent(this, new SceneLoaderEventArgs(scene));
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
        OnSceneLoader(SceneLoaderModel.ProfileScene);
    }

    public void LoadSettingsScene()
    {
        OnSceneLoader(SceneLoaderModel.SettingsScene);
    }
}