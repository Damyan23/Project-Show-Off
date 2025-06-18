using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour
{
    [Header("Pickup Behaviour")]
    [SerializeField] private KeyCode pickupKey;
    [HideInInspector] public bool isItemOnAltar = false;
    private PlayerInteraction playerInteraction;

    void Awake()
    {
        if (InventoryManager.Instance == null) return;

        playerInteraction = PlayerInteraction.Instance;
        pickupKey = playerInteraction.interactWithInteractable;
    }

    public void PickupItem()
    {
        // Add the item to the player's inventory
        if (InventoryManager.Instance != null) { InventoryManager.Instance.AddItem(this.gameObject); }
    }
}
