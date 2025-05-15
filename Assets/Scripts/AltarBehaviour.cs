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

        player = GameObject.FindGameObjectWithTag("Player");
        inventoryManager = InventoryManager.Instance;
    }

    private void Update()
    {
        PauseSoundWhenInRange();
    }

    private void PauseSoundWhenInRange()
    {
        if (player == null || inventoryManager == null) return;
        
        float distance = Vector3.Distance(player.transform.position, inventoryManager.gameObject.transform.position);
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
