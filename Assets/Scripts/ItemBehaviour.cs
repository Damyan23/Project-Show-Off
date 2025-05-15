using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour
{
    [Header("Item Data")]
    public ScriptableObject itemData;


    [Header("Pickup Behaviour")]
    [SerializeField] private KeyCode pickupKey;
    [HideInInspector] public bool isItemOnAltar = false;
    private InventoryManager inventoryManager;

    void Awake()
    {
        if (InventoryManager.Instance == null) return;

        inventoryManager = InventoryManager.Instance;
        pickupKey = inventoryManager.interactWithInteractable;
    }

    public void PickupItem()
    {
        // Add the item to the player's inventory
        if (InventoryManager.Instance != null) { InventoryManager.Instance.AddItem(this.gameObject); }
    }
}
