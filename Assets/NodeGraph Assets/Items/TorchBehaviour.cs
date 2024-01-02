using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchBehaviour : Item
{
    [SerializeField]
    Light spotlight;
    [SerializeField]
    AudioSource click;


    public override GameObject GetEquippedItemObj()
    {

        return gameObject;
    }

    public override void OnEquip()
    {
        gameObject.SetActive(true);
    }

    public override void OnUnequip()
    {
        spotlight.enabled = false;
        gameObject.SetActive(false);
    }

    public override void Use()
    {
        click.Play();
        spotlight.enabled = !spotlight.enabled;
    }


}
