using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarBehaviour : MonoBehaviour
{
    [HideInInspector] public bool isSlotTaken = false;
    [HideInInspector] public GameObject item;
    private AudioSource _audio;
    private GameObject _currentItem;
    void Awake()
    {
        _audio = GetComponent<AudioSource>();
        // Ensure the collider is a trigger for detection
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        // Tag this object as "Altar" so items can detect it
        gameObject.tag = "Altar";
    }

    /// <summary>
    /// Called by the item when it enters/exits the altar trigger.
    /// </summary>
    public void SetSlotTaken(bool taken, GameObject item)
    {
        _currentItem = item;
        _audio.mute = taken;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, InventoryManager.Instance.interactionDistance);
    }
}
