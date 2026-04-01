using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASR_mode1 : MonoBehaviour, IGun
{
    [SerializeField]private string _modename= "ASRmode1";

    public string ModeName { get => _modename; }

   

    public void shoot(Transform gunpoint)
    {
        Debug.Log("ASR_mod1");
    }
}
