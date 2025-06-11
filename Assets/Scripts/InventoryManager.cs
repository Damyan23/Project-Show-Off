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

    public void AddItem(GameObject item)
    {
        currentItem = item;
        rb = currentItem.GetComponent<Rigidbody>();
        rb.isKinematic = true;

        item.transform.SetParent(cam.transform);
        currentItem.transform.rotation = Quaternion.Euler(rotationOffset);

        Vector3 screenCorner = new Vector3(Screen.width, 0, zDepth) + positionOffset;
        Vector3 worldTargetPos = cam.ScreenToWorldPoint(screenCorner);
        currentItem.transform.position = worldTargetPos;

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