using UnityEngine;

public class BrushInteraction : MonoBehaviour, IInteraction
{
    public float sineSpeed = 5f;
    public float sineHeight = 0.15f;
    public float sineOff = 0f;
    public float cosSpeed = 2.5f;
    public float cosHeight = 0.075f;
    public float cosOff = 0.05f;
    public float rotSpeed = 2.0f;
    public float rotHeight = 15.0f;
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

    public void Update()
    {
        if (active)
        {
            transform.GetChild(0).localPosition = new Vector3(Mathf.Sin(Time.time * sineSpeed) * sineHeight + sineOff, Mathf.Cos(Time.time * cosSpeed) * cosHeight + cosOff, 0);
            transform.GetChild(0).localEulerAngles = new Vector3(0, 0, Mathf.Sin(Time.time * rotSpeed) * rotHeight);
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
        transform.GetChild(0).localPosition = Vector3.zero;
        transform.GetChild(0).localEulerAngles = Vector3.zero;
    }
}
