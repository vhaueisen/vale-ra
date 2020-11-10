using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static DataModel;

public class SettingsView : ApplicationElement
{
    public Slider ScaleSlider;
    public Slider RotationSlider;
    public Slider TranslationSlider;
    public Slider ElevationSlider;
    public Slider OcclusionSlider;
    public Slider LightEstimationSlider;
    public Text ScaleValue;
    public Text TranslationValue;
    public Text RotationValue;
    public Text ElevationValue;
    public GameObject OcclusionContainer;
    private volatile bool m_shouldUpdate = false;
    public void UpdateVars()
    {
        if (m_shouldUpdate)
        {
            UserSettings.UserData core = MainApp.coreDataModel.Settings.Core;
            core.ScaleSpeed = ScaleSlider.value;
            core.TranslateSpeed = TranslationSlider.value;
            core.RotateSpeed = RotationSlider.value;
            core.ElevateSpeed = ElevationSlider.value;
            core.IsOcclusion = (OcclusionSlider.value > 0);
            core.EstimatingLight = (LightEstimationSlider.value > 0);
            DataEventArgs eventArgs = new DataEventArgs(DataEventArgs.UpdateEvent, core);
            MainApp.coreDataModel.Settings.OnSettingsEvent(this, eventArgs);
        }
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
            LightEstimationSlider.value = core.EstimatingLight ? 1.0f : -1.0f;
            ReLabel();
        }
    }

    public void UpdateSlider()
    {
        ReLabel();
        UpdateVars();
    }

    private void ReLabel()
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

    public void UpdateLightEstimation()
    {
        LightEstimationSlider.value = LightEstimationSlider.value * -1;
        UpdateVars();
    }

    private void Start()
    {
        LoadVars();
        // if (Debug.isDebugBuild)
        //     OcclusionContainer.SetActive(true);
        StartCoroutine(EnableSave());
    }

    private IEnumerator EnableSave()
    {
        for (int i = 0; i < 5; i++)
            yield return null;
        m_shouldUpdate = true;
        yield break;
    }
    public void Disconnect()
    {
        // MainApp.coreDataModel.Login.OnSettingsEvent(this, new DataModel.DataEventArgs(
        //     DataModel.DataEventArgs.UpdateEvent, new LoginSettings.AuthData()));
        // MainApp.sceneLoaderController.OnSceneLoader(this, new SceneLoaderEventArgs(
        //     SceneLoaderModel.LoginScene));
        // Destroy(MainApp.gameObject);
    }

    public void LoadQuickStart()
    {
        MainApp.footerView.LoadQuickStartScene();
    }
}