using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[RequireComponent(typeof(Rigidbody))]
public class Gun : MonoBehaviour, IThrow
{
    public GunTypeSo guntype;
    private IGun mode1;
    private IGun mode2;
    public IGun currentMode;
    private string Type;
    private float FirerateCount;
    [SerializeField] private Transform GunPoint;
    public int currentAmmo;
    public int AllAmmoleft;
   
   

    private bool Isreload;
    private void Start()
    {
        Type = guntype.GunTypename;
        currentAmmo = guntype.MaxCapacity;
        AllAmmoleft = guntype.MaxAmmoCanTake;
       
        switch (Type)
        {
            case "AssaltRifle":
                

                mode1 = new ASR_mode1();
                mode2 = new ASR_mode2();
                currentMode = mode1;
                break;
            case "Pistol":
            
                mode1 = GetComponent<PistolFistMode>();
                mode2 = GetComponent<PistolSeconMode>();
               
                currentMode = mode1;    
                break;
            case "ShotGun":
                mode1 = GetComponent<ShortGunMode1>();
                mode2 = GetComponent<ShortGUnmode2>();
                currentMode = mode1;
             
                break;
            case "CrossBow":
                mode1 = GetComponent<CrossBowMode1>();
                currentMode = mode1;
                break;
            case "RocketLauncher":
                mode1 = GetComponent<RockeLaunderMode>();
                currentMode = mode1;
                break;
        }
    }
   
    public void OnThrow()
    {
        StartCoroutine(Throwed());
    }
    
  
    public void ExecuteFire()
    {
        if (Isreload) return;
        if (Time.time < FirerateCount) return;
        if (currentAmmo <= 0)
        {
            
            if (AllAmmoleft > 0)
            {
                ReloadFuc();
            }
            return;
        }
        float damageToSend = guntype.Damage;
        bool critState = false; EquimentSlot owner = GetComponentInParent<EquimentSlot>();
        if (owner != null && owner.isCritHundredActive)
        {
            critState = true;
           
            damageToSend = guntype.Damage * 3f;
        }
        currentMode.shoot(GunPoint, guntype, damageToSend, critState);
        FirerateCount = Time.time + guntype.FireRate;
        currentAmmo--;
       

       
    }
    public void SwitchMode()
    {
        if (mode2 == null) return;
        currentMode = (currentMode == mode1) ? mode2 : mode1;
        GameEvent.UpdateAmmo?.Invoke();

    }
    public void ReloadFuc()
    {
        if (Isreload || currentAmmo == guntype.MaxCapacity || AllAmmoleft <= 0) return;
        StartCoroutine(Reloading());
        
    }
   private IEnumerator Reloading()
    {
        Isreload=true;
        yield return new WaitForSeconds(guntype.ReloadTime);
        int ammoNeeded = guntype.MaxCapacity - currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, AllAmmoleft);
        currentAmmo += ammoToReload;
        AllAmmoleft -= ammoToReload;
        
      
        Isreload = false;   
    }
    private IEnumerator Throwed()
    {
        gameObject.layer = 7;
        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        yield return new WaitForSeconds(2f);
        gameObject.layer = 1;
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
    public void Addammo(int amount)
    {
        AllAmmoleft += amount;
      
    }
    public AmmoTypeSO GetAmmoType()
    {
        if (guntype != null)
        {
            return guntype.AmmoType;
        }
        return null;
    }
}
