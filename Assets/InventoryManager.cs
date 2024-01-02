using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    public List<Item> inventory;
    int currentItemIndex = 0;


    // Start is called before the first frame update
    void Start()
    {
        inventory = new List<Item>
        {
            new NoItem()
        };
    }

    public void AddItem(Item newItem)
    {
        inventory.Add(newItem);
    }


    // Update is called once per frame
    void Update()
    {
        ChangeItemCheck();
        UseItemCheck();
    }

    void UseItemCheck()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("using item " + currentItemIndex.ToString());
            inventory[currentItemIndex].Use();
        }
    }

    void ChangeItemCheck()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            inventory[currentItemIndex].OnUnequip();
            currentItemIndex += Input.mouseScrollDelta.y > 0 ? 1 : -1;
        }
        else
        {
            return;
        }


        currentItemIndex = mod(currentItemIndex, inventory.Count);
        Debug.Log(currentItemIndex);

        inventory[currentItemIndex].OnEquip();

    }

    int mod(int k, int n) { return ((k %= n) < 0) ? k + n : k; }
}
