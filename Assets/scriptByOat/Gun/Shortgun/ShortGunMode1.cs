using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortGunMode1 : MonoBehaviour ,IGun
{
    [SerializeField] private string _modename = "Shortgun1";

    [SerializeField]private ShortGunConfig _config;

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


            GameObject bulletObj = Objectpool.Instance.SpawnFromPool("PistolBullet", gunpoint.position, finalRotation);
            if (bulletObj.TryGetComponent<Bullet>(out Bullet bulletScript))
            {
               
                bulletScript.Setup(finalDamage, isCrit);
                bulletScript.OnobjectSpawn();
            }
        }

    }
}
