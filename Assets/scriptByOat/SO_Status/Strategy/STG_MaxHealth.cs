using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STG_MaxHealth : MonoBehaviour, IEffectPickUp
{

    [SerializeField] private float Healtdrop=40f;
    [SerializeField] private float armordrop=40f;
    [SerializeField] private bool IsmaxHealth;
    public void Onpickup(EquimentSlot equid)
    {   
        if (IsmaxHealth)
        {
            float finalhealth = equid.held.status.Health - equid.held._health;
            float finalarmor = equid.held.status.Armor - equid.held._armor;

            equid.held._health += finalhealth;
            equid.held._armor += finalarmor;

        }
        else
        {

            equid.held._health += Healtdrop;
            equid.held._armor += armordrop;
        }
        if (equid.held._health > equid.held.status.Health) equid.held._health = equid.held.status.Health;
        if (equid.held._armor > equid.held.status.Armor) equid.held._armor = equid.held.status.Armor;
       
        GameEvent.UpdatePLayerStatus?.Invoke();
    }
}
