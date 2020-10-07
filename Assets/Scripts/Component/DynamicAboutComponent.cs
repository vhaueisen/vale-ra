using System;
using UnityEngine;
using UnityEngine.UI;

public class DynamicAboutComponent : ApplicationElement
{
    public Text text;

    void Start()
    {
        text.text = string.Format("Versão: {0}\n2019 - {1}", Application.version, DateTime.Now.Year.ToString());
    }
}
