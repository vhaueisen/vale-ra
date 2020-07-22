using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    public Image minaBot;
    public Text dialogContent;
    public RectTransform dialogBodyTransform;
    public RectTransform dialogPanel;
    private float animationTime = 0.75f;
    public GameObject tutorialPanel;
    public RectTransform overlayRect;
    public struct Tips
    {
        public Tips(List<string> content)
        {
            this.content = content;
        }
        public List<string> content;
    }
    public string Content
    {
        get
        {
            return m_content;
        }
        set
        {
            m_content = value;
            dialogContent.text = value;
            dialogPanel.sizeDelta = new Vector2(dialogPanel.sizeDelta.x, value.Length + 35.0f);
        }
    }
    private string m_content;
    // private bool state = false;
    private Tips[] tipsList;
    private bool m_moveToNext = false;
    public Text nextTipperText;
    private void Start()
    {
        List<string> homeTips = new List<string>();
        homeTips.Add("Seja bem vindo ao Vale RA! Você está atualmente na cena de Início. Aqui, é possível visualizar virtualmente os objetos disponíveis em seu Inventário. O inventário guarda todos os objetos disponíveis para visualização.");
        homeTips.Add("ipsum door sit amet");
        tipsList = new Tips[]
        {
            new Tips(homeTips)
        };
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EnterDialog(tipsList[0]);
        }
    }

    public void EnterDialog(Tips eventTip)
    {
        StopAllCoroutines();
        StartCoroutine(TipManager(eventTip));
    }

    private IEnumerator TipManager(Tips eventTip)
    {
        nextTipperText.text = "Próximo";
        tutorialPanel.SetActive(true);
        minaBot.rectTransform.LeanMoveX(50.0f, animationTime).setEase(LeanTweenType.easeInExpo);
        dialogPanel.LeanScale(Vector3.one, animationTime).setEase(LeanTweenType.easeSpring).setDelay(animationTime / 2.0f).setOnStart(
            () => dialogPanel.gameObject.SetActive(true)
        );
        dialogPanel.LeanRotateZ(0.0f, animationTime).setEase(LeanTweenType.easeSpring).setDelay(animationTime / 2.0f);
        overlayRect.LeanAlpha(1.0f, animationTime);
        for (int i = 0; i < eventTip.content.Count; i++)
        {
            if (i == (eventTip.content.Count - 1))
            {
                nextTipperText.text = "Sair";
            }
            Content = eventTip.content[i];
            while (!m_moveToNext)
                yield return null;
            m_moveToNext = false;
        }
        ExitDialog();
    }

    public void ExitDialog()
    {
        minaBot.rectTransform.LeanMoveX(-minaBot.rectTransform.sizeDelta.x - 25.0f, animationTime).setEase(LeanTweenType.easeOutExpo);
        dialogPanel.LeanScale(Vector3.one * 0.01f, animationTime).setEase(LeanTweenType.easeOutExpo);
        dialogPanel.LeanRotateZ(30.0f, animationTime).setEase(LeanTweenType.easeSpring).setOnComplete(
            () =>
            {
                dialogPanel.gameObject.SetActive(false);
                tutorialPanel.SetActive(false);
            }
        );
        overlayRect.LeanAlpha(0.0f, animationTime);
    }

    public void NextTip()
    {
        m_moveToNext = true;
    }
}
