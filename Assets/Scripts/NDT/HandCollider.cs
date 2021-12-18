using UnityEngine;

public class HandCollider : MonoBehaviour
{
    #region Singleton
    private static HandCollider _instance;
    public static HandCollider Instance
    {
        get
        {
            return _instance;
        }

        set
        {
            _instance = value;
        }
    }
    #endregion

    #region ColliderBehaviour
    Collider target = null;
    Interactable interactable = null;
    private IHandTracker tracker;
    #endregion
    private void Start()
    {
        tracker = new KeyboardTracker();
        tracker.Initialize();
    }

    void Update()
    {
        tracker.UpdateTracker();
        transform.position = tracker.Target;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Interactable")
            return;
        collide(other);
    }

    void collide(Collider other)
    {
        if (other != target)
        {
            if (interactable != null)
                interactable.tracker = null;
            target = other;
            interactable = target.GetComponent<Interactable>();
            if (interactable != null)
                interactable.tracker = tracker;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag != "Interactable")
            return;
        target = null;
        if (interactable != null)
            interactable.tracker = null;
        interactable = null;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag != "Interactable")
            return;
        collide(other);
    }
}