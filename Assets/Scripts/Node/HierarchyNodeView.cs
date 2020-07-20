using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NodeViewEventArgs
{
    public NodeViewEventArgs(byte type, HierarchyNodeController node)
    {
        Type = type;
        Node = node;
    }
    public byte Type
    {
        get;
    }

    public HierarchyNodeController Node
    {
        get;
    }
}

public class HierarchyNodeView : MonoBehaviour
{
    public HierarchyNodeController node;
    public Text title;
    public Image background;
    private float beginStart = 0.0f;
    public GameObject expandableIcon;

    public void Set(HierarchyNodeController Node)
    {
        node = Node;
    }

    private void Start()
    {
        title.text = node.nodeName;
        expandableIcon.SetActive(node.childNodes.Count > 0);
    }

    public void BeginTouch()
    {
        StopAllCoroutines();
        StartCoroutine(LerpImageColor(background, Color.white, new Color(0.411765f, 0.745098f, 0.156863f)));
        beginStart = Time.time;
    }

    public void EndTouch()
    {
        StopAllCoroutines();
        background.color = Color.white;
        if (Time.time - beginStart > 0.3f)
            NodeEvent?.Invoke(this, new NodeViewEventArgs(0, node));
        else
        {
            node.Pressed();
        }
    }

    static public IEnumerator LerpImageColor(Image image, Color a, Color b, float speed = 3.0f)
    {
        image.color = a;
        float t = 0.0f;
        yield return new WaitForEndOfFrame();
        while (t <= 1.0f)
        {
            image.color = Color.Lerp(a, b, t);
            t += Time.deltaTime * speed;
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }

    public delegate void NodeViewEventHandler(object sender, NodeViewEventArgs eventArgs);
    public static event NodeViewEventHandler NodeEvent;
}