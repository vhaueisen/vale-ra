using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ProfileView : ApplicationElement
{
    private ProfileModel model;
    void Start()
    {
        model = ProfileApp.profileModel;
    }

    public void InitializePersonalInfo(LoginSettings.AuthData personalData)
    {
        model.emailField.text = personalData.email;
        model.jobIndexField.text = personalData.userName;
        model.locationField.text = personalData.location;
        model.nameField.text = personalData.fullName;
        model.teamField.text = personalData.jobLevel;
    }

    public void EnterSelector()
    {
        model.avatarView.SetParent(model.avatarSelectorParent);
        model.avatarView.anchoredPosition = Vector2.zero;
        model.avatarWindow.Enter();
    }

    public void SaveSelection()
    {
        model.profileController.SaveVariables();
        ExitSelection();
    }

    public void ExitSelection()
    {
        model.avatarView.SetParent(model.profilePanelParent);
        model.avatarView.anchoredPosition = Vector2.zero;
        RefreshPreview(model.avatar);
        model.avatarWindow.Exit();
    }

    public void RefreshPreview(ProfileSettings.ProfileData core)
    {
        RefreshPreview(core.AvatarJobId, core.AvatarGenderId, core.AvatarSkinId, core.AvatarHairId);
    }

    public void RefreshPreview(int jobId, int genderId, int skinId, int hairId)
    {
        model.avatarScrollbar.value = jobId * model.avatarScrollbarRatio;
        model.currentJob = jobId;
        model.jobLabel.text = model.jobNames[jobId];

        model.currentGender = genderId;
        model.genderGroup.transform.GetChild(genderId).GetComponent<Toggle>().isOn = true;

        model.currentSkin = skinId;
        model.skinGroup.transform.GetChild(skinId).GetComponent<Toggle>().isOn = true;

        model.currentHair = hairId;
        model.hairGroup.transform.GetChild(hairId).GetComponent<Toggle>().isOn = true;

    }
    public void ChangeJob(int newId)
    {
        model.currentJob = (int)Mathf.Clamp(model.currentJob + newId, 0, ProfileModel.avatarCount - 1);
        model.jobLabel.text = model.jobNames[model.currentJob];
        ScrollPreview(newId);
    }

    public void ChangeGender()
    {
        model.currentGender = model.genderGroup.ActiveToggles().FirstOrDefault<Toggle>().transform.GetSiblingIndex();
        switch (model.currentGender)
        {
            case 0:
                foreach (Image img in model.femaleImages)
                    img.gameObject.SetActive(true);
                foreach (Image img in model.maleImages)
                    img.gameObject.SetActive(false);
                break;
            case 1:
                foreach (Image img in model.maleImages)
                    img.gameObject.SetActive(true);
                foreach (Image img in model.femaleImages)
                    img.gameObject.SetActive(false);
                break;
        }
    }

    public void ChangeSkin()
    {
        foreach (Image img in model.skinImages)
            img.color = model.skinGroup.ActiveToggles().FirstOrDefault<Toggle>().colors.normalColor;
        model.currentSkin = model.skinGroup.ActiveToggles().FirstOrDefault<Toggle>().transform.GetSiblingIndex();
    }

    public void ChangeHair()
    {
        foreach (Image img in model.hairImages)
            img.color = model.hairGroup.ActiveToggles().FirstOrDefault<Toggle>().colors.normalColor;
        model.currentHair = model.hairGroup.ActiveToggles().FirstOrDefault<Toggle>().transform.GetSiblingIndex();
    }

    private void ScrollPreview(int direction)
    {
        model.avatarScrollbar.value = Mathf.Clamp01(model.avatarScrollbar.value + direction * model.avatarScrollbarRatio);
    }
}