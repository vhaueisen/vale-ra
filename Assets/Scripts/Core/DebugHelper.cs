using System.Collections.Generic;
using UnityEngine;

public class DebugHelper : MonoBehaviour
{
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        this.transform.localScale = Vector3.one;
        List<Renderer> renderList = new List<Renderer>(this.GetComponentsInChildren<Renderer>());
        Renderer instanceRenderer = this.GetComponent<Renderer>();
        if (instanceRenderer != null)
            renderList.Add(instanceRenderer);
        Bounds boundingBox = new Bounds();
        foreach (Renderer r in renderList)
            boundingBox.Encapsulate(r.bounds);
        float YOffset = boundingBox.min.y;
        YOffset -= 0.001f + 0.05f;
        Debug.Log(YOffset);
        Gizmos.DrawCube(transform.position + Vector3.up * YOffset, new Vector3(1.0f, 0.1f, 1.0f));
    }
}
