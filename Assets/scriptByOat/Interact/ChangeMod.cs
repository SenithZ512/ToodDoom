using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMod : MonoBehaviour, Iinteractable
{
    [SerializeField] private EquimentSlot playerInventory;
    public void Interact()
    {
        if (playerInventory == null) return;

        if (playerInventory.CurrentHolding != null)
        {
            Gun gunScript = playerInventory.CurrentHolding.GetComponent<Gun>();
            gunScript.SwitchMode();

            Debug.Log("Switched Mode for: " + playerInventory.CurrentHolding.name);
        }
    }
}
