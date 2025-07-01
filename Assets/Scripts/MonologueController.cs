using UnityEngine;
using Yarn.Unity;
using UnityEngine.Events;

public class MonologueController : MonoBehaviour
{
    [Header("Monologue Node Names")]
    public string forestGeneralLines = "ForestGeneralLines";
    public string altarDialogue = "AltarDialogue";
    public string sanityHighGeneral = "SanityHighGeneral";
    public string gameOverGeneral = "GameOverGeneral";
    public string whiteWomenEncounter = "WhiteWomenEncounter";

    private DialogueRunner dialogueRunner;

    private void Start()
    {
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        if (dialogueRunner == null)
        {
            Debug.LogError("DialogueRunner not found in the scene.");
        }

        TriggerForestGeneral();
    }

    private void StartDialogue(string nodeName)
    {
        if (dialogueRunner == null || dialogueRunner.IsDialogueRunning) return;
        dialogueRunner.StartDialogue(nodeName);

        Debug.Log (dialogueRunner + " started.");   
    }

    // === Public Methods to Trigger Dialogue ===

    public void TriggerForestGeneral()
    {
        StartDialogue(forestGeneralLines);
    }

    public void TriggerAltarDialogue()
    {
        StartDialogue(altarDialogue);
    }

    public void TriggerSanityHigh()
    {
        StartDialogue(sanityHighGeneral);
    }

    public void TriggerGameOver()
    {
        StartDialogue(gameOverGeneral);
    }

    public void TriggerWhiteWomenEncounter()
    {
        StartDialogue(whiteWomenEncounter);
    }
}
