using UnityEngine;
using UnityEngine.UI;

public class Professor : MonoBehaviour
{
    public Animator animator;
    public Color Neutral;
    public Color Confused;
    public Color Excited;
    public Text dialogContent;
    public RectTransform dialogContentTransform;
    private float animationTime = 0.5f;
    private float punchDuration = 2f;
    public Material LoonMaterial;
    public string Content
    {
        get
        {
            return m_content;
        }
        set
        {
            m_content = value;
            dialogContentTransform.sizeDelta = new Vector2(dialogContentTransform.sizeDelta.x, value.Length * 1.44f + 52.0f);
            dialogContentTransform.LeanScale(
                Vector3.zero, animationTime
            ).setOnComplete(
                () =>
                {
                    dialogContentTransform.LeanScale(Vector3.one, animationTime)
                 .setEase(LeanTweenType.easeSpring);
                    dialogContent.text = value;
                }
                );
            LeanTween.rotateAroundLocal(dialogContentTransform, Vector3.up, 360f, animationTime * 2.0f).setFrom(0).setEase(LeanTweenType.easeSpring);
        }
    }
    private string m_content;


    public void ThumbsUp()
    {

    }
    void Start()
    {
        NDTStates.ChangeState.AddListener(OnStateChange);
        OnStateChange(NDTAction.Skip);
    }

    private void OnStateChange(NDTAction to)
    {
        if (NDTStates.Instance.Previous != null && !NDTStates.Instance.Previous.Skipable && NDTStates.Instance.Previous == NDTStates.Instance.Current && to != NDTAction.Skip)
            Alert();
        else if (NDTStates.Instance.Previous != NDTStates.Instance.Current)
            Talk();
    }

    private void Alert()
    {
        Content = NDTStates.Instance.Current.Alert(InteractableObject.LastInteractedWith.Label.ObjName);
        animator.Play("Alert");
        Sentiment(Confused);
    }



    private void Talk()
    {
        Content = NDTStates.Instance.Current.Hint;
        animator.Play(string.Format("Talk{0}", Mathf.CeilToInt(Random.Range(1f, 2f))));
        Sentiment((NDTStates.Instance.Previous == null || NDTStates.Instance.Previous.Skipable) ? Neutral : Excited);
    }
    private void Exit()
    {
        animator.Play("Exit");
    }

    private void Sentiment(Color target)
    {
        LeanTween.value(dialogContentTransform.gameObject, ColorBaloon, LoonMaterial.color, target, punchDuration).setEase(LeanTweenType.punch);
    }

    void ColorBaloon(Color c)
    {
        LoonMaterial.color = c;
    }
}
