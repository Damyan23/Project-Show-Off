using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AltarBehaviour : MonoBehaviour
{
    [Header("Altart settings")]
    [HideInInspector] public bool isSlotTaken = false;
    [SerializeField] private float interactionDistance = 3f;
    [HideInInspector] public GameObject item;

    [Header("Sound settings")]
    [SerializeField] private float intervalInSeconds = 5f;
    [SerializeField] private AudioClip _clip;
    private AudioSource audioSource;
    private float timeSinceLastSound = 0f;

    [Header("References")]
    private GameObject _currentItem = null;
    private GameObject player = null;
    private PlayerInteraction playerInteraction;
    private MonologueController monologueController;

    [Header("Particles settings")]
    private ParticleSystem[] particles;

    void Awake()
    {
        TryGetComponent<AudioSource>(out audioSource);
        monologueController = GameManager.instance.gameObject.GetComponent<MonologueController>();

        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found in the scene.");
        }

        playerInteraction = player.GetComponent<PlayerInteraction>();
        // Debug.Log(playerInteraction);
        particles = GetComponentsInChildren<ParticleSystem>();
        foreach (var particle in particles)
        {
            var main = particle.main;
            particle.Stop();
            main.playOnAwake = false;
        }
    }

    private void Update()
    {
        if (isSlotTaken && audioSource != null)
        {
            if (audioSource.isPlaying) audioSource.Pause();
            return;
        }

        float distance = Vector3.Distance(player.transform.position, transform.GetChild(1).position);
        if (distance <= interactionDistance)
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
        item.transform.SetParent(transform.GetChild(0).transform);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.Euler(Vector3.zero);
        lightCandelsUp();
        monologueController.TriggerAltarItemPlacedGeneral();
    }

    void lightCandelsUp()
    {
        if (particles.Length == 0) return;

        foreach (var particle in particles)
        {
            if (!particle.isPlaying) particle.Play();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere (transform.GetChild(1).position, interactionDistance);
    }
}
