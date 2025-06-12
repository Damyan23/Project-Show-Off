using Unity.VisualScripting;
using UnityEngine;
using Yarn.Unity;

public class ItemMonologueTrigger : MonoBehaviour
{
    public string startNode = "StartMonologue";
    public string itemNode = "NearbyItemMonologue";
    public float triggerDistance = 10f;
    private bool hasTriggered = false;
    private Transform player;
    private Transform startAltar;

    private DialogueRunner dialogueRunner;
    private PlayerInteraction playerInteraction;
    private Collider closestInteractable = null;
    private Transform closestInteractableT;
    
    // Track which item we last showed dialogue for
    private Collider lastItemShownDialogue;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        startAltar = GameObject.FindGameObjectWithTag("Start Altar")?.transform;
        playerInteraction = player.GetComponent<PlayerInteraction>();
        dialogueRunner = FindObjectOfType<DialogueRunner>();

        if (player == null)
            Debug.LogError("Player not found. Make sure it's tagged correctly.");

        if (startAltar == null)
            Debug.LogError("Start altar not found. Make sure it's tagged correctly.");
    }

    private void Update()
    {
        if (player == null || dialogueRunner == null || dialogueRunner.IsDialogueRunning)
            return;

        // Update the transform we have in the function only if the closest interactable from inventory manager actually changes
        if (playerInteraction.closestInteractable != null && playerInteraction.closestInteractable != closestInteractable)
        {
            closestInteractable = playerInteraction.closestInteractable;
            closestInteractableT = playerInteraction.closestInteractable.GetComponent<Transform>();
        }
        
        float distanceToClosestItem = Mathf.Infinity;

        if (closestInteractableT != null)
            distanceToClosestItem = Vector3.Distance(closestInteractableT.position, player.position);

        // Only show dialogue if we're within range AND this is a different item than the last one we showed dialogue for
        if (distanceToClosestItem <= triggerDistance && closestInteractable != lastItemShownDialogue)
        {
            // Set the Yarn variable for substitution
            dialogueRunner.VariableStorage.SetValue("$itemName", closestInteractableT.name);

            // Start the dialogue
            dialogueRunner.StartDialogue(itemNode);
            
            // Remember this item so we don't show dialogue again
            lastItemShownDialogue = closestInteractable;
        }
        
        // Reset the last item if we move away from all items (optional - allows re-triggering if player leaves and comes back)
        if (distanceToClosestItem > triggerDistance)
        {
            lastItemShownDialogue = null;
        }

        if (hasTriggered) return;

        float distanceToFirstAltar = Vector3.Distance(transform.position, player.position);

        if (distanceToFirstAltar <= triggerDistance)
        {
            hasTriggered = true;
            dialogueRunner.StartDialogue(startNode);
        }
    }
}