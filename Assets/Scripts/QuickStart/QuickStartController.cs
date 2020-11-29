using static DataModel;
using UnityEngine.Android;

public class QuickStartController : ApplicationElement
{
    public QuickStartModel model;
    public void Initialize()
    {
        model = FindObjectOfType<QuickStartModel>();
        model.controller = this;
        model.initialPosition = model.indicatorIdx.anchoredPosition;
        model.slideBtns[0].onClick.AddListener(delegate { model.view.SwitchPanel(-1); });
        model.slideBtns[1].onClick.AddListener(delegate { model.view.SwitchPanel(1); });
        LeanTween.delayedCall(1f, () => model.view.EnterHeader());
        LeanTween.delayedCall(1.5f, () => model.view.EnterBody());
    }

    public bool IsCompleted()
    {
        if (MainApp.coreDataModel.Settings.IsLoaded)
        {
            UserSettings.UserData core = MainApp.coreDataModel.Settings.Core;
            return core.quickStartCompleted;
        }
        return false;
    }

    public void Done()
    {
        MainApp.coreDataModel.Settings.Core.quickStartCompleted = true;
        DataEventArgs eventArgs = new DataEventArgs(DataEventArgs.UpdateEvent, MainApp.coreDataModel.Settings.Core);
        MainApp.coreDataModel.Settings.OnSettingsEvent(this, eventArgs);
        MainApp.footerView.LoadHomeScene();
    }

    public bool RequestCamera()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            Permission.RequestUserPermission(Permission.Camera);
        return Permission.HasUserAuthorizedPermission(Permission.Camera);
#else
        return true;
#endif
    }
}