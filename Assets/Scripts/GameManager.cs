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
        }
    #endregion


    [Header("Game over Settings")]
    [SerializeField] private int requiredNumberOfDoneAltars = 3;
    [HideInInspector] public int numberOfDoneAltars = 0;
    [SerializeField] private GameObject babyPrefab;
    [SerializeField] private Vector3 babySpawnPosition;
    private bool babySpawned = false;

    // Define the delegate type
    public delegate void OnAltarDoneEvent();

    // Create an instance of the delegate that other scripts can subscribe to
    public OnAltarDoneEvent onAltarDone;

    void Start()
    {
        onAltarDone += IncrementAltarCount;
    }

    void Update()
    {
        if ((numberOfDoneAltars == requiredNumberOfDoneAltars) && !babySpawned) spawnBaby();
    }

    private void spawnBaby()
    {
        GameObject baby = Instantiate(babyPrefab);
        baby.transform.position = babySpawnPosition;
    }

    private void IncrementAltarCount()
    {
        numberOfDoneAltars++;
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
}
