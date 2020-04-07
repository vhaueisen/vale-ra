using UnityEngine;

public class ProfileController : ApplicationElement
{
    private ProfileModel model;

    private void Start()
    {
        model = ProfileApp.profileModel;
        model.avatarScrollbarRatio = 1.0f / (float)ProfileModel.avatarCount;
        LoadVariables();
        model.avatarWindow.Exit();
    }

    public void SaveVariables()
    {
        ProfileSettings.ProfileData core = new ProfileSettings.ProfileData();
        Debug.Log(string.Format("Saved: {0}", model.currentJob));
        core.AvatarJobId = model.currentJob;
        core.AvatarGenderId = model.currentGender;
        core.AvatarSkinId = model.currentSkin;
        core.AvatarHairId = model.currentHair;
        DataModel.DataEventArgs eventArgs = new DataModel.DataEventArgs(DataModel.DataEventArgs.UpdateEvent, core);
        MainApp.coreDataModel.Profile.OnSettingsEvent(this, eventArgs);
        model.avatar = core;
    }

    public void LoadVariables()
    {
        if (MainApp.coreDataModel.Profile.IsLoaded)
        {
            model.avatar = MainApp.coreDataModel.Profile.Core;
            model.profileView.RefreshPreview(model.avatar);
        }
        Debug.Log(string.Format("Loaded: {0}", model.avatar.AvatarJobId));
    }
}
