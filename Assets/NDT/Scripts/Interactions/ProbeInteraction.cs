using System.Collections;
using UnityEngine;

public class ProbeInteraction : MonoBehaviour, IInteraction
{
    public Vector3 targetPosition;
    public Vector3 targetRotation;
    public float posSpeed;
    public float rotSpeed;
    public float duration = 10f;
    public NDTAction OnComplete;
    [SerializeField] private Texture2D[] m_probeTextures;
    [SerializeField] private Renderer m_probeRenderer;
    private NDTState.ProbeState previousState = NDTState.ProbeState.Default;
    private NDTState.ProbeState currentState;
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

        currentState = NDTStates.Instance.Current.State;
        if (previousState != currentState)
        {
            StartCoroutine(FadeTexture());
        }
    }

    public void Interact()
    {
        Active = true;
    }

    private IEnumerator FadeTexture()
    {
        // float duration = 1f;
        float startTime = Time.time;
        Material probeMaterial = m_probeRenderer.material;
        Texture2D initial = m_probeTextures[(int)previousState];
        Texture2D target = m_probeTextures[(int)currentState];
        // probeMaterial.SetTexture("_Initial", initial);
        // probeMaterial.SetTexture("_Target", target);
        // while (Time.time < startTime + duration)
        // {
        //     float t = (Time.time - startTime) / duration;
        //     probeMaterial.SetFloat("_Value", t);
        //     Debug.Log(t);
        //     yield return null;
        // }
        probeMaterial.SetTexture("_Initial", target);
        probeMaterial.SetTexture("_Target", initial);
        probeMaterial.SetFloat("_Value", 0);
        previousState = currentState;
        yield break;
    }

    private void Reset()
    {
        NDTStates.ChangeState.Invoke(OnComplete);
        T.localPosition = Vector3.zero;
        T.localEulerAngles = Vector3.zero;
        GetComponent<InteractableObject>().state = InteractableObject.InteractableState.idle;
    }
}
