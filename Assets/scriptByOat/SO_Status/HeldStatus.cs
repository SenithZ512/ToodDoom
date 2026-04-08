using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class HeldStatus : MonoBehaviour,IDamgaeAble,IElement
{
    public SO_Status status;
   
    public float _health;
    public float _armor ; 
    public float _speed ;
    private void Start()
    {
        _health = status.Health;
        _armor = status.Armor;
        _speed = status.speed;
        GameEvent.UpdatePLayerStatus?.Invoke();
    }
    public void OnDamaged(float damageAmount)
    {
        Accept(new DamageVisitor(damageAmount));
        Debug.Log($"HP: {_health}, Armor: {_armor}");
       

    }
    private void ExecuteDamage(float amount)
    {
        if (_armor > 0) { _armor -= amount; }
        else { _health -= amount; }
    }

    public void Accept(IVisitor visitor)
    {
       visitor.Visit(this);
    }
}
