using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    #region Singleton
    public static InventoryManager Instance;
    #endregion

    [Header("Settings")]
    [SerializeField] private Vector3 positionOffset;
    
    [SerializeField] public Vector3 rotationOffset;

    private Camera cam;

    [Header("Inventory")]
    [SerializeField] private GameObject itemParent;
    public KeyCode interactWithInteractable = KeyCode.E;
    [SerializeField] private KeyCode dropKey = KeyCode.Q;
    [HideInInspector] public GameObject currentItem;
    [HideInInspector] public bool isSlotTaken = false;
    
    [Header("Interaction Settings")]
    public float interactionDistance = 5f;
    [SerializeField] private LayerMask interactionLayerMask;

    [HideInInspector] public delegate void decreaseSanity();
    public decreaseSanity _decreaseSanity;

    [Header("References")]
    private GameManager gameManager;

    private float zDepth = 1f; 
    private Rigidbody rb;

    void Awake()
    {
        Instance = this;
        cam = Camera.main;

        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        gameManager = GameManager.instance;
    } 

    void Update()
    {
        Collider[] interactabels = Physics.OverlapSphere (this.transform.position, interactionDistance, interactionLayerMask);


        if (Input.GetKeyDown (interactWithInteractable))
        {
            float minDistance = float.MaxValue;
            Collider closestInteractable = null;


            foreach (Collider interactable in interactabels)
            {
                if (!isSlotTaken && interactable.CompareTag("Altar")) continue;
                else if (isSlotTaken && interactable.CompareTag("Item")) continue;

                float distance = Vector3.Distance(interactable.transform.position, this.transform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestInteractable = interactable;
                }
            }

            if (closestInteractable != null) 
            { 
                handleInteractable(closestInteractable); 
            }   
        }

        if (isSlotTaken) { UpdateItemPositionAndRotation(); }
        
        if (Input.GetKeyDown(dropKey)) { dropItem(); }
    }

    void handleInteractable (Collider interactable)
    {
        if (interactable.CompareTag("Item") && !this.isSlotTaken && !interactable.GetComponent<ItemBehaviour>().isItemOnAltar)
        {
            if (Input.GetKeyDown(interactWithInteractable)) { interactable.GetComponent<ItemBehaviour>().PickupItem(); }
        }
        else if (interactable.CompareTag("Altar") && this.isSlotTaken)
        {
            if (Input.GetKeyDown(interactWithInteractable)) { placeItemOnAltar(interactable.gameObject); }
        }
    }

    private void UpdateItemPositionAndRotation()
    {
        if (currentItem == null) return;

        currentItem.transform.localRotation = Quaternion.Euler(rotationOffset);
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

    void dropItem ()
    {
        if (currentItem == null || !isSlotTaken) return;

        rb.isKinematic = false;

        currentItem.transform.SetParent(itemParent.transform);

        isSlotTaken = false;
    }

    void placeItemOnAltar(GameObject altar)
    {
        if (currentItem == null) return;

        currentItem.transform.localPosition = Vector3.zero;
        currentItem.transform.localRotation = Quaternion.identity;

        altar.GetComponent<AltarBehaviour>().PlaceItem(currentItem);

        currentItem.transform.localPosition = Vector3.zero + (altar.GetComponent<MeshRenderer>().bounds.size.y - currentItem.GetComponent<MeshRenderer>().bounds.size.y) * Vector3.up;
        currentItem.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 90));

        rb.isKinematic = false;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.includeLayers += this.gameObject.layer;

        isSlotTaken = false;

        _decreaseSanity.Invoke();
        gameManager.AltarCompleted();
    }
}