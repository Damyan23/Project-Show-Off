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
        item.layer = LayerMask.NameToLayer("HeldItem");
        rb = currentItem.GetComponent<Rigidbody>();
        rb.isKinematic = true;

        item.transform.SetParent(cam.transform);
        // Set local position so it's always at the same spot relative to the camera
        currentItem.transform.localPosition = cam.transform.InverseTransformPoint(
            cam.ViewportToWorldPoint(new Vector3(1f, 0f, zDepth))
        ) + positionOffset;


        // Set local rotation
        currentItem.transform.localRotation = Quaternion.Euler(rotationOffset);

        isSlotTaken = true;
    }


    public void DropItem()
    {
        if (currentItem == null || !isSlotTaken) return;

        rb.isKinematic = false;
        currentItem.transform.SetParent(itemParent.transform);

        isSlotTaken = false;
        currentItem = null;
    }
}