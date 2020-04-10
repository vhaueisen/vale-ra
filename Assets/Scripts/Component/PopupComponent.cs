using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PopupComponent : MonoBehaviour
{
    public Text title;
    public Text content;
    public Image darkOverlay;
    public GameObject container;

    public void Popup(string Title, string Content)
    {
        container.SetActive(true);
        title.text = Title;
        content.text = Content;
        darkOverlay.enabled = true;
        StartCoroutine(ResizeWindow());
    }

    public void Exit()
    {
        container.SetActive(false);
        darkOverlay.enabled = false;
    }

    private IEnumerator ResizeWindow()
    {
        yield return new WaitForEndOfFrame();
        content.text = content.text + " ";
        yield break;
    }

}
