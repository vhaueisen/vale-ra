using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Listens for touch events and performs an AR raycast from the screen touch point.
/// AR raycasts will only hit detected trackables like feature points and planes.
///
/// If a raycast hits a trackable, the <see cref="placedPrefab"/> is instantiated
/// and moved to the hit position.
/// </summary>
[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnPlane : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject m_PlacedPrefab;
    public static UnityEvent<bool> OnAnchor = new UnityEvent<bool>();
    /// <summary>
    /// The prefab to instantiate on touch.
    /// </summary>
    public GameObject placedPrefab
    {
        get { return m_PlacedPrefab; }
        set { m_PlacedPrefab = value; }
    }

    /// <summary>
    /// The object instantiated as a result of a successful raycast intersection with a plane.
    /// </summary>
    public GameObject spawnedObject { get; private set; }
    public ARSessionOrigin origin;
    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
    }

    bool IsAnchored = false;

    public void ToggleAnchor()
    {
        IsAnchored = !IsAnchored;
        PlaceOnPlane.OnAnchor.Invoke(IsAnchored);
    }

    void Update()
    {
        if (IsAnchored)
            return;

        if (m_RaycastManager.Raycast(new Vector2(Screen.width / 2f, Screen.height / 2f), s_Hits, TrackableType.PlaneWithinPolygon))
        {
            // Raycast hits are sorted by distance, so the first one
            // will be the closest hit.
            var hitPose = s_Hits[0].pose;

            if (spawnedObject == null)
            {
                spawnedObject = Instantiate(m_PlacedPrefab, Vector3.zero, Quaternion.identity);
                spawnedObject.GetComponent<NDTController>().Initialize();
            }

            origin.MakeContentAppearAt(spawnedObject.transform, hitPose.position, Quaternion.identity);
            var lookPos = Camera.main.transform.position - spawnedObject.transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            spawnedObject.transform.rotation = rotation;
        }
    }

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    ARRaycastManager m_RaycastManager;
}