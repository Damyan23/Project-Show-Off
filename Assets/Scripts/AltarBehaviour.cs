using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarBehaviour : MonoBehaviour
{
    [HideInInspector] public bool isSlotTaken = false;
    [HideInInspector] public GameObject item;

    [SerializeField] private float intervalInSeconds = 5f;

    [SerializeField] private AudioClip _clip;
    private AudioSource _audio;

    private GameObject _currentItem;
    private GameObject player = null;
    private InventoryManager inventoryManager;

    private float timeSinceLastSound = 0f;

    void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _clip = Resources.Load<AudioClip>("Audio/altar_sound");

        if (_clip == null)
        {
            Debug.LogError("AudioClip not found at Resources/Audio/altar_sound");
        }

        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found in the scene.");
        }

        inventoryManager = InventoryManager.Instance;
    }

    private void Update()
    {
        if (isSlotTaken)
        {
            if (_audio.isPlaying) _audio.Pause();
            return;
        }

        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance <= inventoryManager.interactionDistance)
        {
            if (_audio.isPlaying) _audio.Pause();
            return;
        }

        timeSinceLastSound += Time.deltaTime;
        if (timeSinceLastSound >= intervalInSeconds && _clip != null)
        {
            _audio.PlayOneShot(_clip);
            timeSinceLastSound = 0f;
        }
    }

    public void PlaceItem(GameObject item)
    {
        if (isSlotTaken) return;
        isSlotTaken = true;
        _currentItem = item;
        item.transform.SetParent(transform);
    }
}
