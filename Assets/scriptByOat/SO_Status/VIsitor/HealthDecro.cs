
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthVistor : MonoBehaviour, IVisitor
{
    private float _healAmount;

    public HealthVistor(float amount)
    {
        _healAmount = amount;
    }
    public void Visit(HeldStatus heldstatus)
    {
        
    }
}
