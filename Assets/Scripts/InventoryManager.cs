using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    #region Singleton
    public static InventoryManager Instance;
    #endregion

    [Header("Settings")]
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] public Vector3 rotationOffset;

    [Header("Inventory")]
    [SerializeField] private GameObject itemParent;
    [HideInInspector] public GameObject currentItem;
    [HideInInspector] public bool isSlotTaken = false;

    private Camera cam;
    private float zDepth = 1f;
    private Rigidbody rb;

    void Awake()
    {
        Instance = this;
        cam = Camera.main;
    }

    void OnValidate()
    {
        Instance = this;
        if (currentItem != null && cam != null)
        {
            Vector3 viewportPos = new Vector3(1f, 0f, zDepth);
            Vector3 worldTargetPos = cam.ViewportToWorldPoint(viewportPos);
            currentItem.transform.position = worldTargetPos + positionOffset;
            currentItem.transform.rotation = Quaternion.Euler(rotationOffset);
        }
    }

    public void AddItem(GameObject item)
    {
        currentItem = item;
        rb = currentItem.GetComponent<Rigidbody>();

        rb.excludeLayers = LayerMask.GetMask("Player");
        rb.isKinematic = true;
        
        // Parent it to the camera first
        currentItem.transform.SetParent(cam.transform.GetChild(0));

        currentItem.transform.position = Vector3.zero;
        currentItem.transform.rotation = Quaternion.identity;

        // Now set the LOCAL position and rotation relative to the camera
        float fixedDistanceFromCamera = 1.5f;
        Vector3 viewportPos = new Vector3(1f, 0f, fixedDistanceFromCamera);
        Vector3 worldTargetPos = cam.ViewportToWorldPoint(viewportPos);
        
        // Convert world position to local position relative to camera
        Vector3 localTargetPos = cam.transform.InverseTransformPoint(worldTargetPos + positionOffset);
        
        currentItem.transform.localPosition = localTargetPos;
        currentItem.transform.localRotation = Quaternion.Euler(rotationOffset);

        isSlotTaken = true;
    }



    public void DropItem()
    {
        if (currentItem == null) return;
        if (!isSlotTaken) return;

        rb.isKinematic = false;
        //rb.excludeLayers = 0; // Reset exclude layers to allow physics interactions
        currentItem.transform.SetParent(itemParent.transform);

        isSlotTaken = false;
        currentItem = null;
    }
}