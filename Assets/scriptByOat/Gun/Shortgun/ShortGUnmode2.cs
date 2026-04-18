using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortGUnmode2 : MonoBehaviour, IGun
{
    [SerializeField] private string _modename = "Shortgun2";
    [SerializeField] private ShortGunConfig _config;
    public string ModeName => _modename;

  
    public void shoot(Transform gunpoint, GunTypeSo Gundata, float finalDamage, bool isCrit)
    {

        Gundata.FireRate = _config.Firerate;
        int _pelletCount = _config.pelletCount;
        float _spreadAngle = _config.spreadAngle;
        for (int i = 0; i < _pelletCount; i++)
        {


            Quaternion randomRotation = Quaternion.Euler(
                Random.Range(-_spreadAngle, _spreadAngle),
                Random.Range(-_spreadAngle, _spreadAngle),
                0
            );


            Quaternion finalRotation = gunpoint.rotation * randomRotation;


            Objectpool.Instance.SpawnFromPool("PistolBullet", gunpoint.position, finalRotation);
        }
    }
}
