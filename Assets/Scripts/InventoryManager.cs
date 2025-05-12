using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    #region Singleton
    public static InventoryManager Instance;
    #endregion

    [Header("Settings")]
    [SerializeField] private Vector3 positionOffset;
    
    // Make sure this is public and marked with [SerializeField]
    [SerializeField] public Vector3 rotationOff;

    private Camera cam;

    [Header("Inventory")]
    [SerializeField] private GameObject itemParent;
    public KeyCode interactWithItem = KeyCode.E;
    [SerializeField] private KeyCode dropKey = KeyCode.Q;
    [HideInInspector] public GameObject currentItem;
    [HideInInspector] public bool isSlotTaken = false;
    
    [Header("Interaction Settings")]
    [SerializeField] private float interactionDistance = 5f;
    [SerializeField] private LayerMask interactionLayerMask;

    private float zDepth = 1f; 

    private Rigidbody rb;

    void Awake()
    {
        Instance = this;
        cam = Camera.main;

        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Collider[] interactabels = Physics.OverlapSphere (this.transform.position, interactionDistance, interactionLayerMask);

        foreach (Collider interactable in interactabels)
        {
            if (interactable.CompareTag("Item"))
            {
                if (Input.GetKeyDown(interactWithItem))
                {
                    interactable.GetComponent<ItemBehaviour>().PickupItem();
                }
            }
            else if (interactable.CompareTag("Alatar") && this.isSlotTaken)
            {
                if (Input.GetKeyDown(interactWithItem))
                {
                    
                }
            }
        }

        if (isSlotTaken) UpdateItemPositionAndRotation();
        
        if (Input.GetKeyDown(dropKey))
        {
            dropItem();
        }
    }

    private void UpdateItemPositionAndRotation()
    {
        if (currentItem == null) return;

        currentItem.transform.localRotation = Quaternion.Euler(rotationOff);
    }

    public void AddItem(GameObject item)
    {
        currentItem = item;

        rb = currentItem.GetComponent<Rigidbody>();

        rb.isKinematic = true;

        item.transform.SetParent(cam.transform);
        currentItem.transform.rotation = Quaternion.Euler(rotationOff);

        Vector3 screenCorner = new Vector3(Screen.width, 0, zDepth) + positionOffset;
        Vector3 worldTargetPos = cam.ScreenToWorldPoint(screenCorner);
        currentItem.transform.position = worldTargetPos;

        isSlotTaken = true;
    }

    void dropItem ()
    {
        if (currentItem == null) return;

        rb.isKinematic = false;

        currentItem.transform.SetParent(itemParent.transform);

        isSlotTaken = false;
    }
}