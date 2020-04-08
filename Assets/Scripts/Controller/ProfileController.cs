public class ProfileController : ApplicationElement
{
    private ProfileModel model;

    private void Start()
    {
        model = ProfileApp.profileModel;
        model.avatarScrollbarRatio = 1.0f / (float)(ProfileModel.avatarCount - 1);
        LoadVariables();
        model.avatarWindow.Exit();
    }

    public void SaveVariables()
    {
        ProfileSettings.ProfileData core = new ProfileSettings.ProfileData();
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

        if (MainApp.coreDataModel.Login.IsLoaded)
            model.profileView.InitializePersonalInfo(MainApp.coreDataModel.Login.Core);
    }
}