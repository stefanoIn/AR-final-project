using UnityEngine;

public class PlaceCharacterOnce : MonoBehaviour
{
    public GameObject characterPrefab;

    private bool hasBeenPlaced = false;

    // This MUST match "On Content Placed (GameObject)"
    public void Place(GameObject placedAnchor)
    {
        if (hasBeenPlaced)
            return;

        if (characterPrefab == null)
        {
            Debug.LogError("Character Prefab not assigned");
            return;
        }

        Instantiate(
            characterPrefab,
            placedAnchor.transform.position,
            placedAnchor.transform.rotation,
            placedAnchor.transform
        );

        hasBeenPlaced = true;
    }
}
