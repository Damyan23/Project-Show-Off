using Unity.VisualScripting;
using UnityEngine;
using Yarn.Unity;

public class ItemMonologueTrigger : MonoBehaviour
{
   // public string itemName = "Mysterious Object";
    public string yarnNode = "NearbyItemMonologue";
    public float triggerDistance = 10f;
    private bool hasTriggered = false;
    private Transform player;
    private Transform startAltar;

    private DialogueRunner dialogueRunner;
    private InventoryManager inventoryManager;
    private Collider closestInteractable;
    private Transform closestInteractableT;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        startAltar = GameObject.FindGameObjectWithTag("Start Altar")?.transform;
        inventoryManager = InventoryManager.Instance;
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

        // Update the transform we have in the function only if the closest interactable from inventory manager actaully changes
        if (inventoryManager.closestInteractable != null && inventoryManager.closestInteractable != closestInteractable)
        {
            closestInteractableT = inventoryManager.closestInteractable.GetComponent<Transform>();
        }
        float distanceToClosestItem = Mathf.Infinity;

        if (closestInteractableT != null)
            distanceToClosestItem = Vector3.Distance(closestInteractableT.position, player.position);

        if (distanceToClosestItem <= triggerDistance)
        {
            // Set the Yarn variable for substitution
            dialogueRunner.VariableStorage.SetValue("$itemName", closestInteractableT.name);

            // Start the dialogue
            dialogueRunner.StartDialogue(yarnNode);
        }

        if (hasTriggered) return;

        float distanceToFirstAltar = Vector3.Distance(transform.position, player.position);

        if (distanceToFirstAltar <= triggerDistance)
        {
            hasTriggered = true;

        }
    }
}
