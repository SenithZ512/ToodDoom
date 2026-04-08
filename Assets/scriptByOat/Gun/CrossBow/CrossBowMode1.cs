using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossBowMode1 : MonoBehaviour,IGun
{ 
    [SerializeField] private string _modename = "Pistolmode1";
    public string ModeName => _modename;

    public void shoot(Transform gunpoint, GunTypeSo Gundata)
    {
        Objectpool.Instance.SpawnFromPool("PistolBullet", gunpoint.position, gunpoint.rotation);
    }
}
