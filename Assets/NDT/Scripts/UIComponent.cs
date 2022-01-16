using System;
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

    private bool isLooking { get => InteractableRaycaster.LookingAt != null; }
    private bool isGrabbing { get => InteractableRaycaster.Grabed != null; }
    private bool lookingAtStatic { get => InteractableRaycaster.LookingAt.Static; }
    private bool canGrab { get => (isGrabbing || (isLooking && !lookingAtStatic)) && !InteractableObject.IsInretacting && !State.Current.Skipable; }
    private bool canInteract { get => (isGrabbing || (isLooking && lookingAtStatic)) && !InteractableObject.IsInretacting && !State.Current.Skipable; }
    private bool canSkip { get => State.Current.Skipable && !InteractableObject.IsInretacting; }
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
    }
}
