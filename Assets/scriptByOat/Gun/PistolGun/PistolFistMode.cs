using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolFistMode : MonoBehaviour, IGun
{
    [SerializeField] private string _modename = "Pistolmode1";

    public string ModeName => _modename;

    public void shoot(Transform gunpoint, GunTypeSo Gundata)
    {
        Gundata.FireRate = 0.6f;
        Objectpool.Instance.SpawnFromPool("PistolBullet", gunpoint.position, gunpoint.rotation);
    }
}
