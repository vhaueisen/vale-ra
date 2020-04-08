using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static DataModel;

public class SettingsView : ApplicationElement
{
    public Slider ScaleSlider;
    public Slider RotationSlider;
    public Slider TranslationSlider;
    public Slider ElevationSlider;
    public Slider OcclusionSlider;
    public Text ScaleValue;
    public Text TranslationValue;
    public Text RotationValue;
    public Text ElevationValue;

    public void UpdateVars()
    {
        UserSettings.UserData core = new UserSettings.UserData();
        core.ScaleSpeed = ScaleSlider.value;
        core.TranslateSpeed = TranslationSlider.value;
        core.RotateSpeed = RotationSlider.value;
        core.ElevateSpeed = ElevationSlider.value;
        core.IsOcclusion = (OcclusionSlider.value > 0);
        DataEventArgs eventArgs = new DataEventArgs(DataEventArgs.UpdateEvent, core);
        MainApp.coreDataModel.Settings.OnSettingsEvent(this, eventArgs);
    }

    public void LoadVars()
    {
        if (MainApp.coreDataModel.Settings.IsLoaded)
        {
            UserSettings.UserData core = MainApp.coreDataModel.Settings.Core;
            ScaleSlider.value = core.ScaleSpeed;
            TranslationSlider.value = core.TranslateSpeed;
            RotationSlider.value = core.RotateSpeed;
            ElevationSlider.value = core.ElevateSpeed;
            OcclusionSlider.value = core.IsOcclusion ? 1.0f : -1.0f;
            UpdateSlider();
        }
    }

    public void UpdateSlider()
    {
        ScaleValue.text = string.Format("{0:0}%", ScaleSlider.value * 100.0);
        TranslationValue.text = string.Format("{0:0}%", TranslationSlider.value * 100.0);
        RotationValue.text = string.Format("{0:0}%", RotationSlider.value * 100.0);
        ElevationValue.text = string.Format("{0:0}%", ElevationSlider.value * 100.0);
    }

    public void UpdateOcclusion()
    {
        OcclusionSlider.value = OcclusionSlider.value * -1;
        UpdateVars();
    }

    private void Start()
    {
        LoadVars();
    }

    public void Disconnect()
    {
        MainApp.coreDataModel.Login.OnSettingsEvent(this, new DataModel.DataEventArgs(
            DataModel.DataEventArgs.UpdateEvent, new LoginSettings.AuthData()));
        MainApp.sceneLoaderController.OnSceneLoader(this, new SceneLoaderEventArgs(
            SceneLoaderModel.LoginScene));
        Destroy(MainApp.gameObject);
    }
}