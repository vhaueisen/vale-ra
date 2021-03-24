using System.Linq;
using UnityEngine;

public class InspectionManipulator : MonoBehaviour
{
    public Manipulator[] Manipulators;
    public Material GlowingMaterial;

    public class Manipulator
    {
        public Manipulator(InspectionGroup group, Material GlowingMaterial)
        {
            Group = group;
            Meshes = group.Assets.Select(o => o.GetComponent<MeshRenderer>()).ToArray();
            DefaultMaterials = group.Assets.Select(o => o.GetComponent<MeshRenderer>().materials).ToArray();
            GlowingMaterials = new Material[DefaultMaterials.Length][];
            for (int i = 0; i < DefaultMaterials.Length; i++)
            {
                GlowingMaterials[i] = new Material[DefaultMaterials[i].Length];
                for (int j = 0; j < DefaultMaterials[i].Length; j++)
                    GlowingMaterials[i][j] = GlowingMaterial;
            }
        }
        public InspectionGroup Group;
        public MeshRenderer[] Meshes;
        public Material[][] DefaultMaterials;
        public Material[][] GlowingMaterials;
    }

    public void Initialize(InspectionBundle bundle)
    {
        Manipulators = new Manipulator[bundle.Inspections.Length];
        for (int i = 0; i < bundle.Inspections.Length; i++)
        {
            Manipulators[i] = new Manipulator(bundle.Inspections[i], GlowingMaterial);
        }
    }

    public void Hint(InspectionGroup group, bool off = false)
    {
        for (int i = 0; i < Manipulators.Length; i++)
        {
            if (group == Manipulators[i].Group)
                for (int j = 0; j < Manipulators[i].Meshes.Length; j++)
                {
                    Manipulators[i].Meshes[j].materials = off ? Manipulators[i].DefaultMaterials[j] : Manipulators[i].GlowingMaterials[j];
                }
        }
    }

    public void SetAnomaly(InspectionGroup group, InspectionBundle bundle)
    {
        if (group.MisfortuneIdx >= 0)
        {
            InspectionMisfortune anomaly = bundle.Misfortune[group.MisfortuneIdx];
            switch (anomaly.T)
            {
                case InspectionMisfortune.Type.Default:
                    SetDefaultAnomaly(anomaly);
                    break;
                case InspectionMisfortune.Type.Missing:
                    SetMissingAnomaly(anomaly);
                    break;
            }
        }
    }

    private void SetDefaultAnomaly(InspectionMisfortune anomaly)
    {
        for (int i = 0; i < anomaly.Disable.Length; i++)
            anomaly.Disable[i].SetActive(false);

        for (int i = 0; i < anomaly.Enable.Length; i++)
            anomaly.Enable[i].SetActive(true);
    }

    private void SetMissingAnomaly(InspectionMisfortune anomaly)
    {
        for (int i = 0; i < anomaly.Missing.Length; i++)
            anomaly.Missing[i].SetActive(Random.Range(0f, 1f) > 0.2f);
        anomaly.Missing[(int)Random.Range(0f, anomaly.Missing.Length)].SetActive(false);
    }
}