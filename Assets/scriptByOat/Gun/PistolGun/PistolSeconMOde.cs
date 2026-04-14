using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolSeconMode : MonoBehaviour, IGun
{
    [SerializeField] private string _modename = "PistolMOde2";

    public string ModeName => _modename;

   
   

    public void shoot(Transform gunpoint, GunTypeSo Gundata, float finalDamage, bool isCrit)
    {
       
        Objectpool.Instance.SpawnFromPool("PistolBullet", gunpoint.position, gunpoint.rotation);
    }
}
