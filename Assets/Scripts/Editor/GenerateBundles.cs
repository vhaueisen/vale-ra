using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GenerateBundles : Editor
{
    private const float MinExtends = 0.1f;
    private const float MaxExtends = 10.0f;
    private const float DefaultExtends = 0.8f;


    [MenuItem("/Bundle/Build Windows AssetBundles")]
    static void WindowsBuildAll()
    {
        DoBuild(BuildTarget.StandaloneWindows64);
    }

    [MenuItem("/Bundle/Build Android AssetBundles")]
    static void AndroidBuildAll()
    {
        DoBuild(BuildTarget.Android);
    }

    [MenuItem("/Bundle/Build IOS AssetBundles")]
    static void IOSBuildAll()
    {
        DoBuild(BuildTarget.iOS);
    }

    [MenuItem("Bundle/Calculate Asset Bundle variables")]
    static void CalculateVariables()
    {
        ARObejctModel[] models = FindObjectsOfType<ARObejctModel>();
        foreach (ARObejctModel m in models)
        {
            GameObject instance = Instantiate(m.ARPrefab, Vector3.zero, Quaternion.identity);
            instance.transform.localScale = Vector3.one;
            List<Renderer> renderList = new List<Renderer>(instance.GetComponentsInChildren<Renderer>());
            Renderer instanceRenderer = instance.GetComponent<Renderer>();
            if (instanceRenderer != null)
                renderList.Add(instanceRenderer);
            Bounds boundingBox = new Bounds();
            foreach (Renderer r in renderList)
                boundingBox.Encapsulate(r.bounds);

            float maxDim = boundingBox.size.x;

            if (boundingBox.size.y > maxDim)
                maxDim = boundingBox.size.y;
            if (boundingBox.size.z > maxDim)
                maxDim = boundingBox.size.z;

            m.InitialScaleFactor = Mathf.Round(DefaultExtends * 100.0f / maxDim) / 100.0f;
            m.MaxScaleFactor = Mathf.Round(MaxExtends * 100.0f / maxDim) / 100.0f;
            m.MinScaleFactor = Mathf.Round(MinExtends * 100.0f / maxDim) / 100.0f;

            if (m.MaxScaleFactor < 1.0f)
                m.MaxScaleFactor = 1.0f;

            if (m.MinScaleFactor > 1.0f)
                m.MinScaleFactor = 1.0f;

            if (m.MinScaleFactor <= 0.1f)
                m.MinScaleFactor = 0.1f;

            m.InitialScaleFactor = Mathf.Clamp(m.InitialScaleFactor, m.MinScaleFactor, m.MaxScaleFactor);
            m.YOffset = boundingBox.min.y;
            PrefabUtility.ApplyPrefabInstance(m.gameObject, InteractionMode.AutomatedAction);
            DestroyImmediate(instance);
        }
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