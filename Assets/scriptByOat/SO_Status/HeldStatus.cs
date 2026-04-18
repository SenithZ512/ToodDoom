using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class HeldStatus : MonoBehaviour,IElement
{
    public SO_Status status;
    public float _health;
    public float _armor ; 
    public float _speed ;
    private IOndeath ondeath;
    private void Start()
    {
        _health = status.Health;
        _armor = status.Armor;
        _speed = status.speed;
        GameEvent.UpdatePLayerStatus?.Invoke();
        ondeath = GetComponent<IOndeath>();
    }
    private void Update()
    {
        if (_health <= 0)
        {
            if(ondeath != null) ondeath.OnDeath();

        }
    }

    public void Accept(IVisitor visitor)
    {
       visitor.Visit(this);
    }
}
