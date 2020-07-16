using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using static DataModel;
public class BundleManager : ApplicationElement
{
    public static bool LoadScript(string filepath, out ARObjectScript script)
    {
        AssetBundle bundle = AssetBundle.LoadFromFile(filepath);
        script = new ARObjectScript();
        if (bundle == null)
            return false;

        string folderPath = Path.GetDirectoryName(filepath);
        StringBuilder s = new StringBuilder();
        string manifestPath = Path.Combine(folderPath, bundle.name + ".manifest");
        if (File.Exists(manifestPath))
        {
            string[] text = File.ReadAllLines(manifestPath);
            string addr = "";
            foreach (string t in text)
            {
                if (t.Contains("ARObjectModel.prefab"))
                {
                    addr = t.Substring(2);
                }
            }
            if (addr.Length > 0)
            {
                GameObject asset = bundle.LoadAsset(addr) as GameObject;
                if (asset != null)
                {
                    ARObejctModel _model = asset.GetComponent<ARObejctModel>();
                    if (_model != null)
                    {
                        script.Clone(_model);
                        script.bundlePath = filepath;
                        script.addr = addr;
                        script.id = bundle.name;
                        return true;
                    }
                }
            }
        }
        return false;
    }
}