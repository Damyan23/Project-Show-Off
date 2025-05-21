using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AltarBehaviour : MonoBehaviour
{
    [Header("Altart settings")]
    [HideInInspector] public bool isSlotTaken = false;
    [HideInInspector] public GameObject item;

    [Header("Sound settings")]
    [SerializeField] private float intervalInSeconds = 5f;
    [SerializeField] private AudioClip _clip;
    private AudioSource audioSource;
    private float timeSinceLastSound = 0f;

    [Header("References")]
    private GameObject _currentItem = null;
    private GameObject player = null;
    private InventoryManager inventoryManager;

    [Header("Particles settings")]
    private ParticleSystem[] particles;

    void Awake()
    {
        TryGetComponent<AudioSource>(out audioSource);

        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found in the scene.");
        }

        inventoryManager = InventoryManager.Instance;
        particles = GetComponentsInChildren<ParticleSystem>();
    }

    private void Update()
    {
        if (isSlotTaken && audioSource != null)
        {
            if (audioSource.isPlaying) audioSource.Pause();
            return;
        }

        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance <= inventoryManager.interactionDistance)
        {
            if (audioSource.isPlaying) audioSource.Pause();
            return;
        }

        timeSinceLastSound += Time.deltaTime;
        if (timeSinceLastSound >= intervalInSeconds && _clip != null)
        {
            audioSource.PlayOneShot(_clip);
            timeSinceLastSound = 0f;
        }
    }

    public void PlaceItem(GameObject item)
    {
        if (isSlotTaken) return;
        isSlotTaken = true;
        _currentItem = item;
        item.transform.SetParent(transform);
        lightCandelsUp();
    }

    void lightCandelsUp()
    {
        if (particles.Length == 0) return;

        foreach (var particle in particles)
        {
            if (!particle.isPlaying) particle.Play();
        }
    }
}
