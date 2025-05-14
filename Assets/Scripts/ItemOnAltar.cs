using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemOnAltar : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Only react when colliding with an Altar
        if (other.CompareTag("Altar"))
        {
            // Tell the altar that this item has arrived
            var altar = other.GetComponent<AltarBehaviour>();
            if (altar != null)
                altar.SetSlotTaken(true, gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Altar"))
        {
            var altar = other.GetComponent<AltarBehaviour>();
            if (altar != null)
                altar.SetSlotTaken(false, null);
        }
    }
}