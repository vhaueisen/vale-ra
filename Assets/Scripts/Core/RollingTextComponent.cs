using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RollingTextComponent : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField] private bool m_FreezeX;
    [SerializeField] private bool m_FreezeY;
    [SerializeField] private bool m_FreezeZ;
    [SerializeField] private Transform m_target;
    [SerializeField] private Image m_AlphaMask;
    [SerializeField] private Image m_AlphaMaskReverse;
    [SerializeField] private Color m_RightColor = new Color(105 / 255f, 190 / 255f, 40 / 255f, 1f);
    [SerializeField] private Color m_LeftColor = new Color(187 / 255f, 19 / 255f, 62 / 255f, 1f);
    [SerializeField] private Color m_AlphaMaskDefaultColor = new Color(0, 0, 0, 0);
    [SerializeField] private Color m_AlphaMaskReverseDefaultColor = new Color(0, 0, 0, 0);
    private float m_t;

    public UnityEvent<float> OnDragEnd;
    public void OnDrag(PointerEventData eventData)
    {
        m_target.position = new Vector3(
            m_FreezeX ? m_target.position.x : Input.mousePosition.x,
            m_FreezeY ? m_target.position.y : Input.mousePosition.y,
            m_FreezeZ ? m_target.position.z : Input.mousePosition.z
        );

        m_t = (Input.mousePosition.x - Screen.width) / Screen.width * 2f + 1f;

        if (m_AlphaMask != null)
        {
            Color c = m_t > 0 ? m_RightColor : m_LeftColor;
            m_AlphaMask.color = new Color(c.r, c.g, c.b, c.a * Mathf.Abs(m_t));
            m_AlphaMask.rectTransform.localScale = new Vector3(m_t > 0 ? -1 : 1, 1, 1);
        }

        if (m_AlphaMaskReverse != null)
        {
            m_AlphaMaskReverse.color = new Color(m_AlphaMaskReverseDefaultColor.r, m_AlphaMaskReverseDefaultColor.g, m_AlphaMaskReverseDefaultColor.b, m_AlphaMaskReverseDefaultColor.a * Mathf.Abs(1f - m_t));
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnDragEnd?.Invoke(m_t);
        m_target.localPosition = Vector3.zero;
        m_AlphaMaskReverse.color = m_AlphaMaskReverseDefaultColor;
        m_AlphaMask.color = m_AlphaMaskDefaultColor;
    }
}