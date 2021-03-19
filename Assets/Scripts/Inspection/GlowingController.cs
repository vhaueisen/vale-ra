
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlowingController : MonoBehaviour
{
    struct Asset
    {
        public Asset(GameObject instance, MeshRenderer mesh, Material[] defaultMaterials, Material[] glowingMaterials)
        {
            Instance = instance;
            Mesh = mesh;
            DefaultMaterials = defaultMaterials;
            GlowingMaterials = glowingMaterials;
        }
        public GameObject Instance;
        public MeshRenderer Mesh;
        public Material[] DefaultMaterials;
        public Material[] GlowingMaterials;
    }

    struct AssetGroup
    {
        public AssetGroup(string key, Asset[] assets)
        {
            Key = key;
            Assets = assets;
        }
        public string Key;
        public Asset[] Assets;
    }

    public enum State
    {
        Idle,
        Active,
        Disable
    }

    private string m_key;
    private State currentState = State.Idle;
    private List<AssetGroup> m_assetGroups = new List<AssetGroup>();
    public Material glowingMaterial;

    public IEnumerable<string> Initialize(IEnumerable<string> groups)
    {
        if (currentState == State.Idle)
        {
            foreach (string name in groups)
            {
                GameObject groupParent = GameObject.Find(name);
                if (groupParent != null)
                {
                    int groupLength = groupParent.transform.childCount;
                    Asset[] assets = new Asset[groupLength];
                    for (int i = 0; i < groupLength; i++)
                    {
                        GameObject instance = groupParent.transform.GetChild(i).gameObject;
                        if (name != null)
                        {
                            MeshRenderer mesh = instance.GetComponent<MeshRenderer>();
                            if (mesh != null)
                            {
                                Material[] defaultMaterials = mesh.materials;
                                Material[] glowingMaterials = new Material[defaultMaterials.Length];
                                for (int j = 0; j < defaultMaterials.Length; j++)
                                    glowingMaterials[j] = glowingMaterial;
                                assets[i] = new Asset(instance, mesh, defaultMaterials, glowingMaterials);
                            }
                        }
                    }
                    m_assetGroups.Add(new AssetGroup(name, assets));
                    currentState = State.Disable;
                }
                else Debug.Log(name);

            }
        }
        return m_assetGroups.Select(o => o.Key).ToArray();
    }

    public void ChangeState(State target, string key = "")
    {
        if (currentState == State.Disable && target == State.Active && key != "")
        {
            UpdateMaterials(key, true);
        }

        if (target == State.Disable && currentState == State.Active)
        {
            UpdateMaterials(m_key, false);
        }
        currentState = target;
    }

    private void UpdateMaterials(string key, bool glowing)
    {
        foreach (AssetGroup assets in m_assetGroups)
        {
            if (assets.Key == key)
                foreach (Asset a in assets.Assets)
                    a.Mesh.materials = glowing ? a.GlowingMaterials : a.DefaultMaterials;
            m_key = key;
        }
    }
}