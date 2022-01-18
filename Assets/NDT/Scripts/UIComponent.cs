using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIComponent : MonoBehaviour
{
    public enum UIInput
    {
        Grab,
        Interact,
        Skip
    }

    public void Grab()
    {
        if (isGrabbing)
            InteractableRaycaster.Grabed.Release();
        else if (isLooking)
            InteractableRaycaster.LookingAt.Pick();
    }
    public void Interact()
    {
        if (isGrabbing)
            InteractableRaycaster.Grabed.Interact();
        else if (isLooking && lookingAtStatic)
            InteractableRaycaster.LookingAt.Interact();
    }
    public void Skip() => NDTStates.ChangeState.Invoke(NDTAction.Skip);

    public Button GrabBtn;
    public Button InteractBtn;
    public Button SkipBtn;
    public Text GrabBtnText;
    public Text InteractBtnText;
    public Text SkipBtnText;
    public NDTStates State;
    public TextMeshProUGUI Label;

    private bool isLooking { get => InteractableRaycaster.LookingAt != null; }
    private bool isGrabbing { get => InteractableRaycaster.Grabed != null; }
    private bool lookingAtStatic { get => InteractableRaycaster.LookingAt.Static; }
    private bool canGrab { get => (isGrabbing || (isLooking && !lookingAtStatic)) && !InteractableObject.IsInretacting && !State.Current.Skipable; }
    private bool canInteract { get => (isGrabbing || (isLooking && lookingAtStatic)) && !InteractableObject.IsInretacting && !State.Current.Skipable; }
    private bool canSkip { get => State.Current.Skipable && !InteractableObject.IsInretacting; }
    private bool isFinished { get => State.Current.Action == NDTAction.Finish; }
    private bool hasFinished = false;
    private string grabbedName
    {
        get
        {
            try
            {
                return InteractableRaycaster.Grabed.Label.ObjName;
            }
            catch (NullReferenceException)
            {
                return "";
            }
        }
    }
    private string lookingAtName
    {
        get
        {
            try
            {
                return InteractableRaycaster.LookingAt.Label.ObjName;
            }
            catch (NullReferenceException)
            {
                return "";
            }
        }
    }


    void Start()
    {
        GrabBtn.onClick.AddListener(Grab);
        InteractBtn.onClick.AddListener(Interact);
        SkipBtn.onClick.AddListener(Skip);
    }

    void Update()
    {

        GrabBtnText.text = canGrab ? isGrabbing ? $"Soltar {grabbedName}" : $"Pegar {lookingAtName}" : "";
        GrabBtn.interactable = canGrab;

        InteractBtnText.text = canInteract ? isGrabbing ? $"Interagir com {grabbedName}" : $"Interagir com {lookingAtName}" : "";
        InteractBtn.interactable = canInteract;

        SkipBtnText.text = canSkip ? "Próximo" : "";
        SkipBtn.interactable = canSkip;

        Label.text = lookingAtName;
        if (isFinished && !hasFinished) Finish();

    }
    public GameObject TrophyObject;
    public Transform TrophyTransform;
    void Finish()
    {
        hasFinished = true;
        TrophyObject.SetActive(true);
        TrophyObject.LeanScale(Vector3.one, 1f).setEase(LeanTweenType.easeSpring);
        LeanTween.rotateAroundLocal(TrophyTransform.gameObject, Vector3.up, 360f, 5f).setFrom(0).setEase(LeanTweenType.linear).setRepeat(-1);
        TrophyTransform.LeanMoveLocalY(0.1f, 2.5f).setLoopPingPong().setEase(LeanTweenType.easeInBounce);
    }
}
