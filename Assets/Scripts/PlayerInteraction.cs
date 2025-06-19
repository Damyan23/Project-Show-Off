using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    #region Singleton
    public static PlayerInteraction Instance;
    #endregion


    [Header("Interaction Settings")]
    public float interactionDistance = 50f;
    [SerializeField] private LayerMask interactionLayerMask;
    public KeyCode interactWithInteractable = KeyCode.E;
    [SerializeField] private KeyCode dropKey = KeyCode.Q;
    [SerializeField] private Vector3 placementOffset;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI pressFToInteract;

    private GameManager gameManager;
    [HideInInspector] public Collider closestInteractable = null;

    public delegate void decreaseSanity();
    public decreaseSanity _decreaseSanity;

    void Start()
    {
        Instance = this;
        gameManager = GameManager.instance;
    }

    void Update()
    {
        FindClosestInteractable();

        if (closestInteractable != null && !pressFToInteract.enabled) pressFToInteract.enabled = true;
        else if (closestInteractable == null && pressFToInteract.enabled) pressFToInteract.enabled = false;

        if (Input.GetKeyDown(interactWithInteractable))
        {
            if (closestInteractable != null)
            {
                HandleInteractable(closestInteractable);
            }
            else if (pressFToInteract.enabled) { pressFToInteract.enabled = false; }
        }

        if (Input.GetKeyDown(dropKey))
        {
            InventoryManager.Instance.DropItem();
        }
    }

    void FindClosestInteractable()
    {
        Collider[] interactables = Physics.OverlapSphere(this.transform.position, interactionDistance, interactionLayerMask);

        float minDistance = float.MaxValue;
        closestInteractable = null;

        foreach (Collider interactable in interactables)
        {
            // If NOT holding an item, skip altars
            if (!InventoryManager.Instance.isSlotTaken && interactable.CompareTag("Altar")) continue;
            // If holding an item, skip items
            if (InventoryManager.Instance.isSlotTaken && interactable.CompareTag("Item")) continue;
            // If item is on altar, skip it
            if (interactable.CompareTag("Item") && interactable.GetComponent<ItemBehaviour>().isItemOnAltar) continue;

            float distance = Vector3.Distance(interactable.transform.position, this.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestInteractable = interactable;
            }
        }

        // Debug.Log(closestInteractable);
    }

    void HandleInteractable(Collider interactable)
    {
        if (interactable.CompareTag("Item") && !InventoryManager.Instance.isSlotTaken && !interactable.GetComponent<ItemBehaviour>().isItemOnAltar)
        {
            interactable.GetComponent<ItemBehaviour>().PickupItem();
        }
        else if (interactable.CompareTag("Altar") && !interactable.GetComponentInParent<AltarBehaviour>().isSlotTaken && InventoryManager.Instance.isSlotTaken)
        {
            PlaceItemOnAltar(interactable.transform.parent.gameObject);
        }
    }

    void PlaceItemOnAltar(GameObject altar)
    {
        var currentItem = InventoryManager.Instance.currentItem;
        if (currentItem == null) return;

        // Parent to altar first!
        altar.GetComponentInParent<AltarBehaviour>().PlaceItem(currentItem);

        // Center on altar
        //Vector3 verticalOffset = currentItem.transform.parent.GetComponent<MeshRenderer>().bounds.extents.y * Vector3.up;
        currentItem.transform.localPosition = Vector3.zero + placementOffset;
        currentItem.transform.localRotation = Quaternion.Euler(Vector3.zero);

        // Freeze physics
        var rb = currentItem.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        InventoryManager.Instance.isSlotTaken = false;
        InventoryManager.Instance.currentItem = null;

        _decreaseSanity?.Invoke();
        gameManager.AltarCompleted();
        currentItem.GetComponent<ItemBehaviour>().isItemOnAltar = true;
    }
}
