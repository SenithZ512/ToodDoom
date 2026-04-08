using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatus 
{
    float Health {  get; }
    float Armour { get; }
    float Speed { get; } 
}
