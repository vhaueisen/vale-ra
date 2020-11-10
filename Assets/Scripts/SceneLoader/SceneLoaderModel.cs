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
    // public static GameScene LoginScene = new GameScene("Login", 0);
    // public static GameScene ProfileScene = new GameScene("Perfil", 0);
    public static GameScene LoaderScene = new GameScene("Carregando", 0);
    public static GameScene HomeScene = new GameScene("Início", 1);
    public static GameScene QRScene = new GameScene("Leitor QR", 2);
    public static GameScene ARScene = new GameScene("Vale RA", 3);
    public static GameScene SettingsScene = new GameScene("Configurações", 4);
    public static GameScene QuickStartScene = new GameScene("Início Rápido", 5);
    public static GameScene BundleScene = new GameScene("Bundle Manager", 10);
    public static GameScene CurrentScene = LoaderScene;
    public Text headerText;
    public GameObject inventoryPanel;
    public RectTransform loadingPanel;
    public Slider progressBar;
    public RectTransform books;
    public Sprite[] bookSpriteSheet;
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