using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarBehaviour : MonoBehaviour
{
    [HideInInspector] public bool isSlotTaken = false;
    [HideInInspector] public GameObject item;

    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, InventoryManager.Instance.interactionDistance);
    }
}
