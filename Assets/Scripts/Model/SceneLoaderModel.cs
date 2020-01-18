#pragma warning disable 0168
#pragma warning disable 0219
#pragma warning disable 0414

using UnityEngine;
using UnityEngine.UI;

public class SceneLoaderModel : MonoBehaviour
{
    // Static fields naming each scene
    public static GameScene LoaderScene = new GameScene("Carregando", 0);
    public static GameScene HomeScene = new GameScene("Início", 1);
    public static GameScene QRScene = new GameScene("Leitor QR", 2);
    public static GameScene ARScene = new GameScene("Vale RA", 3);
    public static GameScene ProfileScene = new GameScene("Perfil", 4);
    public static GameScene SettingsScene = new GameScene("Configurações", 5);
    public static GameScene BundleScene = new GameScene("Bundle Manager", 6);

    // The Header title to change every new scene
    public Text headerText;

    // The Inventory Panel needs to show its graphics only on the home and AR scene 
    public GameObject inventoryPanel;
    public GameObject loadingPanel;
    public Slider progressBar;

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