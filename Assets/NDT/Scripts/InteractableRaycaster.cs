using UnityEngine;

public class InteractableRaycaster : Singleton<InteractableRaycaster>
{
    public static float Period = 0.3f;
    private int current = 0;
    public static InteractableObject LookingAt;
    public static InteractableObject Grabed;
    private bool raycasting = false;

    void Update()
    {
        if (Time.time / Period > current && raycasting)
        {
            current++;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 10f))
            {
                InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
                if (interactable)
                {
                    interactable.Hover = true;
                    LookingAt = interactable;
                }
            }
        }
    }

    void Start()
    {
        PlaceOnPlane.OnAnchor.AddListener(onAnchor);
    }

    void onAnchor(bool b)
    {
        raycasting = b;
    }
}
