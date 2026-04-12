using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockeLaunderMode : MonoBehaviour,IGun
{
    [SerializeField] private string _modename = "RocketLauncher";
    public string ModeName => _modename;

   
  

    public void shoot(Transform gunpoint, GunTypeSo Gundata, float finalDamage, bool isCrit)
    {
        Objectpool.Instance.SpawnFromPool("RocketBullet", gunpoint.position, gunpoint.rotation);
    }
}
