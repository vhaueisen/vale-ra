using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GenerateBundles : Editor
{

    [MenuItem("/Bundle/ Build Windows AssetBundles")]
    static void WindowsBuildAll()
    {
        DoBuild(BuildTarget.StandaloneWindows64);
    }

    [MenuItem("/Bundle/ Build Android AssetBundles")]
    static void AndroidBuildAll()
    {
        DoBuild(BuildTarget.Android);
    }

    [MenuItem("/Bundle/ Build IOS AssetBundles")]
    static void IOSBuildAll()
    {
        DoBuild(BuildTarget.iOS);
    }


    static void DoBuild(BuildTarget target)
    {
        string dataPath = EditorUtility.SaveFolderPanel(
            "Save all project Bundles",
            "",
            "Bundles");
        BuildPipeline.BuildAssetBundles(dataPath,
            BuildAssetBundleOptions.ChunkBasedCompression,
            target);
    }
}