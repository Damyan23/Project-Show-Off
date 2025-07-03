using System.Collections;
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

    [Header("Enemy Spawn Settings")]
    [SerializeField] private GameObject enemyContainer;
    [SerializeField] private GameObject enemyPrefab;
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
        baby.transform.SetParent(crib.transform.GetChild(0));
        baby.transform.localPosition = Vector3.zero;
        baby.transform.localRotation = Quaternion.Euler (new Vector3 (90f, 0f, -90f));
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

    public void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab is not assigned in GameManager.");
            return;
        }

        // Randomly pick left (-1) or right (1)
        int side = Random.Range(0, 2) * 2 - 1;

        // Generate a random deviation angle within a small cone towards the chosen side
        float baseAngle = side * 45f; // Base angle: -45 (left), 45 (right)
        float angleOffset = Random.Range(-20f, 20f); // Small deviation around base
        float angle = baseAngle + angleOffset;

        // Get direction vector
        Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
        float distance = Random.Range(minDistance, maxDistance);
        Vector3 spawnPos = playerTransform.position + direction.normalized * distance;

        // Adjust spawn Y to align with NavMesh ground
        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(spawnPos, out hit, 10f, UnityEngine.AI.NavMesh.AllAreas))
        {
            spawnPos = hit.position;
        }

        // Calculate enemy height using Renderer bounds
        float enemyHeight = 2f; // Default fallback height
        Renderer rend = enemyPrefab.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            enemyHeight = rend.bounds.size.y;
        }

        // Spawn below the ground
        spawnPos.y -= ariseHeight + enemyHeight;

        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, enemyContainer.transform);

        StartCoroutine(AriseEnemy(enemy, ariseHeight + enemyHeight));
    }



    private IEnumerator AriseEnemy(GameObject enemy, float height)
    {
        Vector3 start = enemy.transform.position;
        Vector3 end = start + Vector3.up * height;
        float elapsed = 0f;
        float duration = height / ariseSpeed;

        while (elapsed < duration)
        {
            enemy.transform.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        enemy.transform.position = end;
    }
    
    private Vector3 GetGroundPos(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up * 10f, Vector3.down, out hit, Mathf.Infinity))
        {
            return hit.point;
        }
        return position; // Fallback to original position if no ground found
    }
}
