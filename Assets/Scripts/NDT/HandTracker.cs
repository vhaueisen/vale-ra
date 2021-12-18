using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTracker : MonoBehaviour
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

    private TrackingInfo tracking;
    [SerializeField] private float speed = 1f;
    public Vector3 currentPosition;
    Transform holdTransform;
    Vector3 startPosition;
    private ManoGestureTrigger grab;
    private ManoGestureTrigger click;
    private string interactableTag = "Interactable";
    private bool colliding = false;
    private Collider target = null;
    private bool holding = false;
    private float lastGrab = 0f;
    private Interactable interactable;
    private bool isGrabbing
    {
        get => ManomotionManager.Instance.Hand_infos[0].hand_info.gesture_info.mano_gesture_trigger == grab;
    }
    private bool isPinch
    {
        get => ManomotionManager.Instance.Hand_infos[0].hand_info.gesture_info.mano_gesture_trigger == click;
    }

    private void Start()
    {
        Initialize();
    }

    void Update()
    {
        UpdatePosition();
        HangleGrab();

        if (holding)
        {
            float step = speed * Time.deltaTime;
            holdTransform.position = Vector3.MoveTowards(holdTransform.position, transform.position, step);
        }
    }
    void UpdatePosition()
    {
        tracking = ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info;
        currentPosition = Camera.main.ViewportToWorldPoint(new Vector3(tracking.palm_center.x, tracking.palm_center.y, tracking.depth_estimation));
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, currentPosition, step);
    }

    private void Initialize()
    {
        grab = ManoGestureTrigger.GRAB_GESTURE;
        click = ManoGestureTrigger.PICK;
    }


    void OnTriggerEnter(Collider other)
    {
        colliding = true;
        target = other;
        interactable = target.GetComponent<Interactable>();
        //interactable.OnCollision(colliding);
    }

    void OnTriggerExit(Collider other)
    {
        colliding = false;
        target = null;
        //interactable.OnCollision(colliding);
        interactable = null;
    }

    private void HangleGrab()
    {
        if (!colliding || !isGrabbing || target == null)
            return;

        if (lastGrab + 5f < Time.time)
        {
            lastGrab = Time.time;
            holding = !holding;
            //interactable.OnGrab(holding);
            holdTransform = holding ? target.transform : null;
        }

    }
}