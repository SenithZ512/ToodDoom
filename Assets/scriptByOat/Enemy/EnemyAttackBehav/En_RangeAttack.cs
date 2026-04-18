using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class En_RangeAttack : MonoBehaviour, IAttackBehaviour
{
    private Gun _gun;
    private AimatPlayer aim;
    private void Awake()
    {
        
        _gun = GetComponentInChildren<Gun>();
        aim = GetComponentInChildren<AimatPlayer>();
        
    }
    public void Attack()
    {
        aim.Aim();
        _gun.ExecuteFire();
        _gun.currentAmmo += 1;
    }
}
