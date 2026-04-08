using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageVisitor : IVisitor
{
    private float _dmgamount;
   
    public DamageVisitor (float amount)
    {
        _dmgamount = amount;
    }
    public void Visit(HeldStatus heldstatus)
    {

        if (heldstatus._armor > 0)
        {
            if (heldstatus._armor >= _dmgamount)
            {
               
                heldstatus._armor -= _dmgamount;
            }
            else
            {
               
                float remainingDamage = _dmgamount - heldstatus._armor;
                heldstatus._armor = 0;
                heldstatus._health -= remainingDamage;
            }
        }
        else
        {
            
            heldstatus._health -= _dmgamount;
        }
        GameEvent.UpdatePLayerStatus?.Invoke();
        Debug.Log("Healt " + heldstatus._health+"armor "+ heldstatus._armor);
    }
   
}
