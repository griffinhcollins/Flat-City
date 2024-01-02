using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public abstract void Use();
    public abstract void OnEquip();
    public abstract void OnUnequip();

    public abstract GameObject GetEquippedItemObj();



}
