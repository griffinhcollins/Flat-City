using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoItem : Item
{
    public override GameObject GetEquippedItemObj()
    {
        return new GameObject("Empty Hands");
    }

    public override void OnEquip()
    {
        
    }

    public override void OnUnequip()
    {

    }

    public override void Use()
    {
        Debug.Log("used empty");
    }

}
