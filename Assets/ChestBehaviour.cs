using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestBehaviour : Interactable
{

    [SerializeField]
    public GameObject openChestPrefab;
    public GameObject treasurePrefab;

    GameObject spawnedTreasure;

    bool beenOpened = false;
    bool takenTreasure = false;

    public override void Activate()
    {
        if (!beenOpened)
        {
            beenOpened = true;
            foreach (Transform childObj in transform)
            {
                Destroy(childObj.gameObject);
            }
            Instantiate(openChestPrefab, transform.position, transform.rotation, transform);
            spawnedTreasure = Instantiate(treasurePrefab, transform.position, transform.rotation);
        }
        else
        {
            if (!takenTreasure)
            {
                takenTreasure = true;
                Transform camTransform = References.player.parent.GetComponentInChildren<Camera>().transform;
                References.player.parent.GetComponent<InventoryManager>().AddItem(spawnedTreasure.GetComponent<Item>());
                spawnedTreasure.transform.parent = camTransform;
                spawnedTreasure.transform.localRotation = Quaternion.Euler(0,90,0);
                spawnedTreasure.transform.localPosition = (Vector3.forward + Vector3.down + Vector3.left) * 0.4f;
                spawnedTreasure.SetActive(false);
            }
        }
        
    }

}
