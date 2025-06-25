using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ItemBehaviour : MonoBehaviour
{
    [Header("Pickup Behaviour")]
    [SerializeField] private KeyCode pickupKey;
    [HideInInspector] public bool isItemOnAltar = false;
    private PlayerInteraction playerInteraction;
    private TaskManager taskManager;

    void Start()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError ("InventoryManager is not initialized. Make sure it is present in the scene before ItemBehaviour.");
            return;
        }
        playerInteraction = PlayerInteraction.Instance;
        pickupKey = playerInteraction.interactWithInteractable;

        taskManager = GameManager.instance.taskManager;
    }

    public void PickupItem()
    {
        // Add the item to the player's inventory
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.AddItem(this.gameObject);

        taskManager.UpdateTask(" Put the <i>" + this.gameObject.name + "</i> on one of the altars.");
    }
}
