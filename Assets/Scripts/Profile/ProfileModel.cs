using UnityEngine;
using UnityEngine.UI;

public class ProfileModel : ApplicationElement
{
    public ProfileSettings.ProfileData avatar;
    public WindowComponent avatarWindow;
    public ProfileController profileController;
    public ProfileView profileView;
    public Text nameField;
    public Text jobIndexField;
    public Text locationField;
    public Text teamField;
    public Text emailField;
    public RectTransform avatarView;
    public Scrollbar avatarScrollbar;
    public RectTransform profilePanelParent;
    public RectTransform avatarSelectorParent;
    public readonly string[] jobNames = {
                                          "Operação de Trem",
                                          "Operação do Centro de Controle",
                                          "Manutenção de Material Rodante",
                                          "Operação de Locotrator",
                                          "Manutenção de Bordo",
                                          "Operação de Pátios e Terminais",
                                          "Manutenção de Via Permanente",
                                          "Operação de equipamentos de Via Permanente",
                                          "Manutenção de Eletroeletrônica",
                                          "Operação de Rodoferroviário"
                                        };
    public Text jobLabel;
    public ToggleGroup genderGroup;
    public Image[] femaleImages;
    public Image[] maleImages;
    public ToggleGroup skinGroup;
    public Image[] skinImages;
    public ToggleGroup hairGroup;
    public Image[] hairImages;
    public float avatarScrollbarRatio;
    public const int avatarCount = 10;
    public int currentJob;
    public int currentHair;
    public int currentSkin;
    public int currentGender;
}