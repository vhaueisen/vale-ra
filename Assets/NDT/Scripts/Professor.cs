using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Professor : MonoBehaviour
{
    public Animator animator;
    public Color Neutral;
    public Color Confused;
    public Color Excited;
    public TextMeshPro dialogContent;
    public Transform dialogTransform;
    public Transform thumbsTransform;
    private float animationTime = 0.5f;
    private float punchDuration = 3f;
    public Material LoonMaterial;
    public Material ThumbsMaterial;
    public string Content
    {
        get
        {
            return m_content;
        }
        set
        {
            m_content = value;
            dialogTransform.LeanScale(
                Vector3.zero, animationTime
            ).setOnComplete(
                () =>
                {
                    dialogTransform.LeanScale(Vector3.one, animationTime)
                 .setEase(LeanTweenType.easeSpring);
                    dialogContent.text = value;
                }
                );
            LeanTween.rotateAroundLocal(dialogTransform.gameObject, Vector3.up, 360f, animationTime * 2.0f).setFrom(0).setEase(LeanTweenType.easeSpring);
        }
    }
    private string m_content;


    public void ThumbsUp(bool up)
    {
        thumbsTransform.LeanScale(up ? new Vector3(1f, 1f, 1f) : new Vector3(1f, 1f, -1f), animationTime).setDelay(2f * animationTime)
                         .setEase(LeanTweenType.easeSpring).setOnComplete(() =>
                         {
                             thumbsTransform.LeanScale(Vector3.zero, animationTime)
                         .setEase(LeanTweenType.easeOutExpo).setDelay(animationTime * 5f);
                         });
        //thumbsTransform.LeanRotateY(0, animationTime).setFrom(-30).setEase(LeanTweenType.easeSpring).setDelay(0.5f * animationTime);
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
        ThumbsUp(false);
        Content = NDTStates.Instance.Current.Alert(InteractableObject.LastInteractedWith.Label.ObjName);
        animator.Play("Alert");
        Sentiment(Confused);
    }



    private void Talk()
    {
        Content = NDTStates.Instance.Current.Hint;
        if (NDTStates.Instance.Previous != null && !NDTStates.Instance.Previous.Skipable) ThumbsUp(true);
        animator.Play(string.Format("Talk{0}", Mathf.CeilToInt(Random.Range(1f, 2f))));
        Sentiment((NDTStates.Instance.Previous == null || NDTStates.Instance.Previous.Skipable) ? Neutral : Excited);
    }
    private void Exit()
    {
        animator.Play("Exit");
    }

    private void Sentiment(Color target)
    {
        LeanTween.value(dialogTransform.gameObject, ColorBaloon, Color.white, target, punchDuration).setEase(LeanTweenType.punch).setDelay(1.5f * animationTime);
    }

    void ColorBaloon(Color c)
    {
        LoonMaterial.color = c;
        ThumbsMaterial.color = c;
    }
}
