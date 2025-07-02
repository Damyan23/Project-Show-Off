using System.Collections;
using UnityEngine;
using Yarn.Unity;

[System.Serializable]
public struct MonologueTime
{
    public float MinTime;
    public float MaxTime;
    public bool CanTrigger;
}

public class MonologueController : MonoBehaviour
{
    [Header("References")]
    private SanityController sanityController;

    [Header("Monologue Node Names")]
    public string forestGeneralLines = "ForestGeneralLines";
    public string sanityHighGeneral = "SanityHighGeneral";
    public string gameOverGeneral = "GameOverGeneral";
    public string whiteWomenEncounter = "WhiteWomenEncounter";

    [Header("Altar Specific Monologues")]
    public string altarLookGeneral = "AltarLookGeneral";
    public string altarItemPlacedGeneral = "AltarItemPlacedGeneral";

    [Header("General Monologue Settings")]
    [SerializeField] private MonologueTime forestGeneralMonologueTime;
    public MonologueTime encounterMonologueTime;
    [Header("Alatr Monologue Settings")]
    [SerializeField] private MonologueTime altarLookMonologueTime;

    [Header("Insanity Monologue Settings")]
    [SerializeField] private MonologueTime insanityMonologueTime;
    [SerializeField] private float insanityThreshold = 60f;  // Can adjust in Inspector

    private DialogueRunner dialogueRunner;

    private void Start()
    {
        sanityController = GameObject.FindGameObjectWithTag("Player").GetComponent<SanityController>();

        dialogueRunner = FindObjectOfType<DialogueRunner>();
        if (dialogueRunner == null)
        {
            Debug.LogError("DialogueRunner not found in the scene.");
            return;
        }

        StartCoroutine(ForestGeneralRoutine());
        StartCoroutine(AltarLookRoutine());
        StartCoroutine(InsanityMonologueRoutine());  // Start the insanity monologue coroutine

        sanityController.OnEnemyDetected += OnEnemyDetectedHandler;
        encounterMonologueTime.CanTrigger = true; // Initialize encounter monologue trigger
    }

    private void Update()
    {
        // Check if encounter monologue can be triggered
        if (encounterMonologueTime.CanTrigger && !dialogueRunner.IsDialogueRunning)
        {
            encounterMonologueTime.CanTrigger = false; // Reset trigger
            StartCoroutine(EncounterMonologueRoutine());
        }
    }


    private void StartDialogue(string nodeName)
    {
        if (dialogueRunner == null || dialogueRunner.IsDialogueRunning) return;
        dialogueRunner.StartDialogue(nodeName);
        Debug.Log($"{nodeName} dialogue started.");
    }

    // === Public Methods to Trigger Dialogue ===

    public void TriggerForestGeneral() => StartDialogue(forestGeneralLines);
    public void TriggerSanityHigh() => StartDialogue(sanityHighGeneral);
    public void TriggerGameOver() => StartDialogue(gameOverGeneral);
    public void TriggerWhiteWomenEncounter() => StartDialogue(whiteWomenEncounter);
    public void TriggerAltarLookGeneral() => StartDialogue(altarLookGeneral);
    public void TriggerAltarItemPlacedGeneral() => StartDialogue(altarItemPlacedGeneral);


    private IEnumerator ForestGeneralRoutine()
    {
        while (true)
        {
            float waitMinutes = Random.Range(forestGeneralMonologueTime.MinTime, forestGeneralMonologueTime.MaxTime)  * 60f;
            yield return new WaitForSeconds(waitMinutes);

            if (!dialogueRunner.IsDialogueRunning)
            {
                TriggerForestGeneral();
            }
        }
    }

    private void OnEnemyDetectedHandler()
    {
        StartCoroutine(ForestGeneralRoutine());
    }

    private IEnumerator EncounterMonologueRoutine()
    {
        while (true)
        {
            encounterMonologueTime.CanTrigger = false; // Prevent multiple triggers
            TriggerWhiteWomenEncounter();
            float waitMinutes = Random.Range(encounterMonologueTime.MinTime, encounterMonologueTime.MaxTime) * 60f;
            yield return new WaitForSeconds(waitMinutes);
            encounterMonologueTime.CanTrigger = true;
        }
    }

    private IEnumerator AltarLookRoutine()
    {
        while (true)
        {
            float waitMinutes = Random.Range(altarLookMonologueTime.MinTime, altarLookMonologueTime.MaxTime) * 60;
            yield return new WaitForSeconds(waitMinutes);

            if (IsAltarInView() && !dialogueRunner.IsDialogueRunning)
            {
                TriggerAltarLookGeneral();
            }
        }
    }

    private bool IsAltarInView()
    {
        Camera playerCamera = Camera.main;
        if (playerCamera == null) return false;

        GameObject[] altars = GameObject.FindGameObjectsWithTag("Altar");
        foreach (var altar in altars)
        {
            Vector3 viewportPos = playerCamera.WorldToViewportPoint(altar.transform.position);

            bool isInView = viewportPos.z > 0 &&     // in front of camera
                            viewportPos.x >= 0 && viewportPos.x <= 1 &&  // inside horizontal viewport
                            viewportPos.y >= 0 && viewportPos.y <= 1;    // inside vertical viewport

            if (isInView)
            {
                return true; // at least one altar is in view
            }
        }

        return false; // no altar in view
    }
    
    private IEnumerator InsanityMonologueRoutine()
    {
        while (true)
        {
            if (!dialogueRunner.IsDialogueRunning && sanityController.CurrentInsanity > insanityThreshold)
            {
                TriggerSanityHigh();
            }
            float waitMinutes = Random.Range(insanityMonologueTime.MinTime, insanityMonologueTime.MaxTime) * 60f;
            yield return new WaitForSeconds(waitMinutes);
        }
    }
}
