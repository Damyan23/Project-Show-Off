using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance;

    void Awake()
    {
        // If there is already an instance and it's not this one
        if (instance != null && instance != this)
        {
            // Destroy this duplicate instance
            Destroy(gameObject);
            return;
        }

        // Set this as the singleton instance
        instance = this;

        // Make the GameManager persistent between scene loads (optional)
        DontDestroyOnLoad(gameObject);
        taskManager = this.GetComponent<TaskManager>();
    }
    #endregion

    private Transform playerTransform;
    [Header("Game over Settings")]
    [SerializeField] private int requiredNumberOfDoneAltars = 3;
    [HideInInspector] public int numberOfDoneAltars = 0;
    [SerializeField] private GameObject babyPrefab;
    [SerializeField] private GameObject crib;

    [Header("Task Settings")]
    [HideInInspector] public TaskManager taskManager;
    private bool babySpawned = false;
    [SerializeField] private AudioClip babySpawnedSound;
    private AudioSource cribAudioSource;

    // Define the delegate type
    public delegate void OnAltarDoneEvent();

    // Create an instance of the delegate that other scripts can subscribe to
    public OnAltarDoneEvent onAltarDone;

    [Header("White Woman Spawn Settings")]
    [SerializeField] private GameObject whiteWomanPrefab;
    [SerializeField] private float minDistance = 8f;
    [SerializeField] private float maxDistance = 15f;
    [SerializeField] private float ariseHeight = 2f;
    [SerializeField] private float ariseSpeed = 2f;


    void Start()
    {
        taskManager.UpdateTask(" Find an item and pick it up");
        onAltarDone += IncrementAltarCount;
        cribAudioSource = crib.AddComponent<AudioSource>();
        crib.GetComponent<AudioSource>().clip = babySpawnedSound;
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if ((numberOfDoneAltars == requiredNumberOfDoneAltars) && !babySpawned) spawnBaby();
    }

    private void spawnBaby()
    {
        taskManager.UpdateTask("Find your baby");
        GameObject baby = Instantiate(babyPrefab);
        baby.transform.position = crib.transform.GetChild(0).position;
        crib.GetComponent<AudioSource>().Play();
        babySpawned = true;
    }

    private void IncrementAltarCount()
    {
        numberOfDoneAltars++;

        string taskText = " Altars completed: " + numberOfDoneAltars + "/" + requiredNumberOfDoneAltars;
        taskManager.UpdateTask(taskText);

        Debug.Log("Altar completed! Total: " + numberOfDoneAltars + "/" + requiredNumberOfDoneAltars);
    }

    public void AltarCompleted()
    {
        // Check if anyone is subscribed to the event before invoking
        if (onAltarDone != null)
        {
            onAltarDone.Invoke();
        }
    }
    
    public void SpawnWhiteWoman()
    {
        if (whiteWomanPrefab != null)
        {
            // Random angle: -45° (left) to +45° (right) relative to forward
            float angle = Random.Range(-45f, 45f);
            float distance = Random.Range(minDistance, maxDistance);

            // Direction vector
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            Vector3 spawnPos = playerTransform.position + direction.normalized * distance;

            // Spawn below ground
            spawnPos.y -= ariseHeight;

            GameObject ww = Instantiate(whiteWomanPrefab, spawnPos, Quaternion.identity, this.transform);

            // Start coroutine to move up
            StartCoroutine(AriseWhiteWoman(ww, ariseHeight));
        }
        else
        {
            Debug.LogError("White woman prefab is not assigned in PlayerInteraction.");
        }
    }

    private IEnumerator AriseWhiteWoman(GameObject ww, float height)
    {
        Vector3 start = ww.transform.position;
        Vector3 end = start + Vector3.up * height;
        float elapsed = 0f;
        float duration = height / ariseSpeed;

        while (elapsed < duration)
        {
            ww.transform.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        ww.transform.position = end;
    }
}
