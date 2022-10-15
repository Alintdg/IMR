using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnPlane : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Instantiates this prefab on a plane at the touch location.")]
    GameObject m_PlacedPrefab;

    UnityEvent placementUpdate;

    [SerializeField]
    GameObject visualObject;

    [SerializeField]
    private int maxPrefabSpawnCount = 20;
    private int placedPrefabCount;
    static List<GameObject> placedPrefabs = new List<GameObject>();

    //for triggering the animations
    private Animator m_Animator;

    public GameObject placedPrefab
    {
        get { return m_PlacedPrefab; }
        set { m_PlacedPrefab = value; }
    }

    public GameObject spawnedObject { get; private set; }

    void Awake()
    {
        //placement
        m_RaycastManager = GetComponent<ARRaycastManager>();

        if (placementUpdate == null)
            placementUpdate = new UnityEvent();

        placementUpdate.AddListener(DisableVisual);

        //animation
        m_Animator = gameObject.GetComponent<Animator>();
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.GetTouch(0).phase == TouchPhase.Began && Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }

    void Update()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
        {

            var hitPose = s_Hits[0].pose;

            if (placedPrefabCount < maxPrefabSpawnCount)
            {
                SpawnPrefab(hitPose);
            }

        }
    }

    private void SpawnPrefab(Pose hitPose)
    {
        spawnedObject = Instantiate(m_PlacedPrefab, hitPose.position, hitPose.rotation * Quaternion.Euler(0f, 180f, 0f));

        placedPrefabs.Add(spawnedObject);
        placedPrefabCount++;

        ActivateInteraction(spawnedObject);

    }


    private void ActivateInteraction(GameObject spawnedObject)
    {
        foreach (GameObject obj in placedPrefabs)
        {
            float distance = Vector3.Distance(spawnedObject.transform.position, obj.transform.position);
            if (distance > 0 && distance < 0.3)
            {
                TriggerAnimation(spawnedObject, "TAttack");
                TriggerAnimation(obj, "TAttack");
            }
        }
    }


    private void TriggerAnimation(GameObject spawnedObject, string triggerType)
    {
        m_Animator = spawnedObject.GetComponent<Animator>();

        if (m_Animator != null)
        {
            m_Animator.SetTrigger(triggerType);
            m_Animator.Play(triggerType);

        }
    }

    public void DisableVisual()
    {
        visualObject.SetActive(false);
    }

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    ARRaycastManager m_RaycastManager;
}