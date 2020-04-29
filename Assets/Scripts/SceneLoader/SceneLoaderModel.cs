#pragma warning disable 0168
#pragma warning disable 0219
#pragma warning disable 0414

using System;
using UnityEngine;
using UnityEngine.UI;

public class SceneLoaderEventArgs : EventArgs
{
    public SceneLoaderEventArgs(SceneLoaderModel.GameScene scene)
    {
        Scene = scene;
    }
    public SceneLoaderModel.GameScene Scene;
}

public class SceneLoaderModel : ApplicationElement
{
    // Static fields naming each scene
    public static GameScene LoginScene = new GameScene("Login", 0);
    public static GameScene LoaderScene = new GameScene("Carregando", 1);
    public static GameScene HomeScene = new GameScene("Início", 2);
    public static GameScene QRScene = new GameScene("Leitor QR", 3);
    public static GameScene ARScene = new GameScene("Vale RA", 4);
    public static GameScene ProfileScene = new GameScene("Perfil", 5);
    public static GameScene SettingsScene = new GameScene("Configurações", 6);
    public static GameScene BundleScene = new GameScene("Bundle Manager", 7);
    public static GameScene CurrentScene = LoaderScene;
    // The Header title to change every new scene
    public Text headerText;

    // The Inventory Panel needs to show its graphics only on the home and AR scene 
    public GameObject inventoryPanel;
    public GameObject loadingPanel;
    public Slider progressBar;
    public Animator locomotiveAnimator;

    // Custom type that holds the scene index and name
    public struct GameScene
    {
        public GameScene(string sceneName_, byte sceneIndex_)
        {
            sceneIndex = sceneIndex_;
            sceneName = sceneName_;
        }

        public byte sceneIndex;
        public string sceneName;
    }
}