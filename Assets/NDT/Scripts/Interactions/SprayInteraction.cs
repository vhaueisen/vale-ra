using UnityEngine;

public class SprayInteraction : MonoBehaviour, IInteraction
{
    public float height = 0.3f;
    public float distance = 0.015f;
    public float speed = 5f;
    public float sineOff = 0f;
    public float sineSpeed = 2.5f;
    public float sineHeight = 0.075f;
    public float rotSpeed = 2.0f;
    public float rotHeight = 15.0f;
    public float duration = 10f;
    public float cookTime = 2f;
    public Color sprayColor;
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
    private ParticleSystem.EmissionModule emission;

    public void Awake()
    {
        ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
        emission = ps.emission;
        var main = ps.main;
        main.startColor = sprayColor;
    }

    public void Update()
    {
        if (active)
        {
            emission.enabled = Time.time > activeTime + cookTime;
            transform.GetChild(0).localPosition = Vector3.Lerp(
                transform.GetChild(0).localPosition,
                new Vector3(Mathf.Sin(Time.time * sineSpeed) * sineHeight, height, distance),
                Time.deltaTime * speed
                );
            transform.GetChild(0).localEulerAngles = new Vector3(35f, 0, Mathf.Sin(Time.time * rotSpeed) * rotHeight);
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
        emission.enabled = false;
        transform.GetChild(0).localPosition = Vector3.zero;
        transform.GetChild(0).localEulerAngles = Vector3.zero;
    }
}
