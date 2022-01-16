using UnityEngine;

public class CanInteraction : MonoBehaviour, IInteraction
{
    public Transform target;
    public Vector3 targetOffset;
    public float posSpeed;
    public float sineSpeed = 5f;
    public float sineHeight = 0.15f;
    public float cosSpeed = 2.5f;
    public float cosHeight = 0.075f;
    public float rotSpeed = 2.0f;
    public float duration = 10f;
    public float startTime = 0f;
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
            Vector3 currentP = T.position;
            Vector3 currentR = T.eulerAngles;
            T.position = Vector3.Lerp(currentP, target.position + targetOffset + T.forward * Mathf.Sin(Time.time * sineSpeed) * sineHeight, Time.deltaTime * posSpeed);
            T.eulerAngles = new Vector3(Mathf.Clamp((Time.time - startTime - 90 / rotSpeed) * rotSpeed, -90f, 30f) - Mathf.Cos(Time.time * cosSpeed) * cosHeight, 90f, -90f);

            if (Time.time > activeTime + duration)
                Active = false;
        }
    }

    public void Interact()
    {
        startTime = Time.time;
        T.SetParent(null);
        Active = true;
    }

    private void Reset()
    {
        NDTStates.ChangeState.Invoke(OnComplete);
        T.SetParent(transform);
        T.localPosition = Vector3.zero;
        T.localEulerAngles = Vector3.zero;
    }
}
