using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortGunMode1 : MonoBehaviour ,IGun
{
    [SerializeField] private string _modename = "Shortgun1";

    public string ModeName => _modename;

    public void shoot(Transform gunpoint)
    {
       
    }
}
