using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGun 
{
    string ModeName { get; }
    void shoot(Transform gunpoint,GunTypeSo Gundata);
  
}
