using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun Type", menuName = "Shooter/Gun Type")]
public class GunTypeSo : ScriptableObject
{
    public string GunTypename;
    public AmmoTypeSO AmmoType;
    public float Damage;
    public float FireRate;
    public int MaxCapacity;
    public int MaxAmmoCanTake;
    public float ReloadTime = 2f;
}
