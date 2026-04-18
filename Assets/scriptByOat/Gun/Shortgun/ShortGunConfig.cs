using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Gun Type", menuName = "Shooter/Shortgun Config Type")]
public class ShortGunConfig : ScriptableObject
{
    public int pelletCount;
    public float spreadAngle;
    public float Firerate =0.8f;
}
