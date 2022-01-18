using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FooterController : ApplicationElement
{
    private void Start()
    {
        FooterView.SceneLoaderEvent += OnSceneLoader;
    }

    public void OnSceneLoader(object sender, SceneLoaderEventArgs sceneLoaderEvent)
    {
        for (int i = 0; i < MainApp.footerModel.ButtonArray.Length; i++)
        {
            if (i == (sceneLoaderEvent.Scene.sceneIndex - 1))
                MainApp.footerModel.ButtonArray[i].interactable = false;
            else
                MainApp.footerModel.ButtonArray[i].interactable = true;
        }
    }
}
