using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHolderLogic : MonoBehaviour
{
    void Start()
    {
        var rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    /*private void OnTriggerEnter(Collider other)
    {
        Card cardScript = other.GetComponent<Card>();

        // Only glue the card if it's ready to be glued.
        if (cardScript != null && cardScript.isReadyToGlue)
        {
            GlueCardToHolder(other.gameObject);
            cardScript.ResetGlueState(); // Reset the state once glued.
        }
    }

    private void GlueCardToHolder(GameObject card)
    {
        card.transform.position = transform.position + new Vector3(0.01f, 0, 0); // Adjust the position as needed

        // Set a specific rotation for the card
        // Example: Make the card stand upright by rotating it 90 degrees around the X axis
        card.transform.rotation = Quaternion.Euler(90, 0, 0);

        // Or, if you want to maintain the holder's rotation and only adjust the card's "tilt",
        // you could do something like this:
        // card.transform.rotation = transform.rotation * Quaternion.Euler(90, 0, 0);

        card.transform.SetParent(transform, worldPositionStays: true);

        var dragObject = card.GetComponent<DragObject>();
        if (dragObject != null)
        {
            dragObject.isGlued = true; // Disable dragging
        }

        Debug.Log("Card glued to holder.");
    }*/

}
