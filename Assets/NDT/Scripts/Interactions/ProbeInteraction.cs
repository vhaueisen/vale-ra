using UnityEngine;

public class ProbeInteraction : MonoBehaviour, IInteraction
{
    public Vector3 targetPosition;
    public Vector3 targetRotation;
    public float posSpeed;
    public float rotSpeed;
    public float duration = 10f;
    public NDTAction OnComplete;
    private bool Active
    {
        get => active;
        set
        {
            active = value;
            if (active)
            {
                activeTime = Time.time;
            }
            else
            {
                Reset();
            }
        }
    }
    private bool active = false;
    private float activeTime;
    private Transform T;
    private void Start()
    {
        T = transform.GetChild(0);
    }

    private void Update()
    {
        if (active)
        {
            Vector3 currentP = T.localPosition;
            Vector3 currentR = T.localRotation.eulerAngles;
            T.localPosition = Vector3.Lerp(currentP, targetPosition, Time.deltaTime * posSpeed);
            T.localRotation = Quaternion.Euler(Vector3.Lerp(currentR, targetRotation, Time.deltaTime * rotSpeed));
            if (Time.time > activeTime + duration)
                Active = false;
        }
    }

    public void Interact()
    {
        Active = true;
    }

    private void Reset()
    {
        NDTStates.ChangeState.Invoke(OnComplete);
        T.localPosition = Vector3.zero;
        T.localEulerAngles = Vector3.zero;
        GetComponent<InteractableObject>().state = InteractableObject.InteractableState.idle;
    }
}
