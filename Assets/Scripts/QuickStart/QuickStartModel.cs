using UnityEngine;
using UnityEngine.UI;

public class QuickStartModel : ApplicationElement
{
    public RectTransform indicatorIdx;
    public RectTransform header;
    public RectTransform[] panelList;
    public RectTransform container;
    private int m_currentIdx = 0;
    public Vector2 initialPosition;
    private const int offset = 30;
    public int maxIdx = 6;
    public Button[] slideBtns;
    public Text nextBtnLabel;
    public QuickStartView view;
    public QuickStartController controller;
    public RectTransform books;
    public Sprite[] bookSpriteSheet;
    public Text currentGuideText;
    public int currentIdx
    {
        get => m_currentIdx;
        set
        {
            m_currentIdx = m_currentIdx + value;
            if (m_currentIdx < 0)
                m_currentIdx = 0;
            else if (m_currentIdx > maxIdx)
                m_currentIdx = maxIdx;
            slideBtns[0].interactable = m_currentIdx > 0;
            nextBtnLabel.text = (m_currentIdx == maxIdx) ? "Concluído" : "Avançar";
        }
    }
    public Vector2 currentPosition
    {
        get
        {
            return initialPosition + Vector2.right * offset * m_currentIdx;
        }
    }
}