using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public bool Static = false;
    private float speed = 2f;
    private float debounceTime = InteractableRaycaster.Period * 1.1f;
    private float debounce;
    public bool hover = false;
    public ToastLabel Label;
    private Vector3 startPosition;
    private IInteraction interaction;
    public static InteractableObject LastInteractedWith;

    private Vector3 target
    {
        get
        {
            switch (state)
            {
                case InteractableState.Grabbed:
                    return InteractableRaycaster.Instance.transform.position - InteractableRaycaster.Instance.transform.forward * 0.1f;
                case InteractableState.Animating:
                    return ProbeObject.Instance.transform.position + Vector3.up * 0.05f;
                default:
                    return startPosition;
            }
        }
    }
    public bool Hover
    {
        get => hover;
        set
        {
            hover = value;
            debounce = Time.time;
            Label.label.SetActive(Hover && state == InteractableState.idle);
            if (InteractableRaycaster.LookingAt == this && !hover)
                InteractableRaycaster.LookingAt = null;
        }
    }

    public enum InteractableState
    {
        idle, Hover, Grabbed, Animating
    }

    public InteractableState state = InteractableState.idle;
    public static bool IsInretacting;
    void Awake()
    {
        Label = GetComponent<ToastLabel>();
        startPosition = transform.position;
        interaction = GetComponent<IInteraction>();
        NDTStates.ChangeState.AddListener(OnAnimationComplete);
    }


    void Update()
    {
        if (Time.time - debounce > debounceTime)
            Hover = false;

        if (!Static)
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * speed);

        Highlight();
    }

    public void Pick()
    {
        if (Static)
            return;

        if (InteractableRaycaster.LookingAt == this && InteractableRaycaster.Grabed == null && !IsInretacting)
        {
            state = InteractableState.Grabbed;
            InteractableRaycaster.Grabed = this;
        }
    }

    public void Release()
    {
        if (Static)
            return;

        if (state == InteractableState.Animating)
            return;

        state = InteractableState.idle;
        InteractableRaycaster.Grabed = null;
    }

    public void Interact()
    {
        if (state == InteractableState.Animating)
            return;

        IsInretacting = true;
        state = InteractableState.Animating;
        interaction.Interact();
        LastInteractedWith = this;
    }

    private void OnAnimationComplete(NDTAction a)
    {
        if (a == NDTAction.Skip)
            return;

        IsInretacting = false;
        state = InteractableState.Grabbed;
        Release();
    }

    bool previousState = false;
    private void Highlight()
    {
        bool newState = Hover && state == InteractableState.idle;
        if (newState != previousState)
        {
            previousState = newState;
            foreach (Transform t in gameObject.GetComponentsInChildren<Transform>(true))
            {
                t.gameObject.layer = newState ? 6 : 0;
            }
        }
    }
}
