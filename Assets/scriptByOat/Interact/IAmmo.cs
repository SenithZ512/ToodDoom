using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAmmo 
{
    AmmoTypeSO AmmoType { get; }
    int AmmoAmount { get; set; }
    void TakeTheAmmo();
}
