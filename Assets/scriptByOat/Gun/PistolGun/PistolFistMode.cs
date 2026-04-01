using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolFistMode : MonoBehaviour, IGun
{
    [SerializeField] private string _modename = "Pistolmode1";

    public string ModeName => _modename;

    

    public void shoot(Transform gunpoint)
    {
        Debug.Log("Pistolmode1");
    }
}
