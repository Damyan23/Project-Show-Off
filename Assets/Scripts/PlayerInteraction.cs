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

            float distance = Vector3.Distance(interactable.transform.position, this.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestInteractable = interactable;
            }
        }

        Debug.Log(closestInteractable);
    }

    void HandleInteractable(Collider interactable)
    {
        if (interactable.CompareTag("Item") && !InventoryManager.Instance.isSlotTaken && !interactable.GetComponent<ItemBehaviour>().isItemOnAltar)
        {
            interactable.GetComponent<ItemBehaviour>().PickupItem();
        }
        else if (interactable.CompareTag("Altar") && InventoryManager.Instance.isSlotTaken)
        {
            PlaceItemOnAltar(interactable.gameObject);
        }
    }

    void PlaceItemOnAltar(GameObject altar)
    {
        var currentItem = InventoryManager.Instance.currentItem;
        if (currentItem == null) return;

        currentItem.transform.localPosition = Vector3.zero;
        currentItem.transform.localRotation = Quaternion.identity;

        altar.GetComponentInParent<AltarBehaviour>().PlaceItem(currentItem);

        currentItem.transform.localPosition = Vector3.zero + (altar.GetComponent<MeshRenderer>().bounds.size.y - currentItem.GetComponent<MeshRenderer>().bounds.size.y) * Vector3.up;
        currentItem.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 90));

        var rb = currentItem.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        InventoryManager.Instance.isSlotTaken = false;
        InventoryManager.Instance.currentItem = null;

        _decreaseSanity?.Invoke();
        gameManager.AltarCompleted();
    }
}
