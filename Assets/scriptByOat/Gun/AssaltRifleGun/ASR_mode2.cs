using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASR_mode2 : MonoBehaviour, IGun
{
    [SerializeField] private string _modename = "ASRmode2";

    public string ModeName => _modename;

   

    public void shoot(Transform gunpoint, GunTypeSo Gundata, float finalDamage, bool isCrit)
    {
        Debug.Log("ASR_mod2");
    }
}
