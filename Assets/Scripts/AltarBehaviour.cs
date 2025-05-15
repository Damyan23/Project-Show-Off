using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarBehaviour : MonoBehaviour
{
    [HideInInspector] public bool isSlotTaken = false;
    [HideInInspector] public GameObject item;
    private AudioSource _audio;
    private GameObject _currentItem;

    private GameObject player = null;
    private InventoryManager inventoryManager;
    void Awake()
    {
        _audio = GetComponent<AudioSource>();

        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found in the scene.");
        }

        inventoryManager = InventoryManager.Instance;
    }

    private void Update()
    {
        PauseSoundWhenInRange();
    }

    private void PauseSoundWhenInRange()
    {
        if (player == null) return;
        
        float distance = Vector3.Distance(player.transform.position, this.transform.position);

        Debug.Log($"Distance to player: {distance}");
        if (distance <= inventoryManager.interactionDistance)
        {
            _audio.Pause();
        }
        else if (!_audio.isPlaying)
        {
            _audio.Play();
        }
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
