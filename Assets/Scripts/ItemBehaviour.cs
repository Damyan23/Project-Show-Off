using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour
{
    [Header("Item Data")]
    public ScriptableObject itemData;


    [Header("Pickup Behaviour")]
    [SerializeField] private float pickupDistance = 5f;
    [SerializeField] private KeyCode pickupKey;

    private InventoryManager inventoryManager;


    void Awake()
    {
        inventoryManager = InventoryManager.Instance;
        pickupKey = inventoryManager.interactWithItem;
    }

    public void PickupItem()
    {
        // Add the item to the player's inventory
        if (InventoryManager.Instance != null) { InventoryManager.Instance.AddItem(this.gameObject); }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupDistance);
    }
}
