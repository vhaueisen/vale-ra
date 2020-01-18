using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToastNotificationComponent : MonoBehaviour
{
    public GameObject content;
    public Image background;
    public Text contentText;
    public RectTransform rect;

    public void Notify(string str)
    {
        StopAllCoroutines();
        background.color = new Color(0.0f, 0.0f, 0.0f, 0.5f);
        contentText.color = Color.white;
        content.SetActive(true);
        contentText.text = str;
        Resize(str.Length);
        StartCoroutine("Exit");
    }

    private void Resize(int textSize)
    {
        if (textSize > 25)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = new Vector2(1.0f, 0.0f);

        }
        else
        {
            rect.anchorMin = new Vector2(0.2f, 0.0f);
            rect.anchorMax = new Vector2(0.8f, 0.0f);
        }
    }

    private IEnumerator Exit()
    {
        yield return new WaitForSeconds(5.0f);
        for (int i = 50; i > 0; i--)
        {
            background.color = new Color(0.0f, 0.0f, 0.0f, i / 100.0f);
            contentText.color = new Color(1.0f, 1.0f, 1.0f, (2 * i) / 100.0f);
            yield return new WaitForSeconds(0.005f);
        }
        content.SetActive(false);
        yield break;
    }
}
