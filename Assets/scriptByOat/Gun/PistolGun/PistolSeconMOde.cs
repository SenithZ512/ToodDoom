using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolSeconMode : MonoBehaviour,IGun
{
    [SerializeField] private string _modename = "PistolMOde2";

    public string ModeName => _modename;

   

    public void shoot(Transform gunpoint)
    {
        Debug.Log("pistol Secondmode");
    }

    
}
