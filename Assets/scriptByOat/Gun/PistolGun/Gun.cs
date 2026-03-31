using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[RequireComponent(typeof(Rigidbody))]
public class Gun : MonoBehaviour, IThrow
{
    [SerializeField]private GunTypeSo guntype;
    private IGun mode1;
    private IGun mode2;
    public IGun currentMode;
    private string Type;
    private float FirerateCount;
    [SerializeField] private Transform GunPoint;
    public int currentAmmo;
    public int AllAmmoleft;
    public Action OnAmmoChanged;
    private void Start()
    {
        Type = guntype.GunTypename;
        currentAmmo = guntype.MaxCapacity;
        AllAmmoleft = guntype.MaxAmmoCanTake;
        Debug.Log("Currentammo"+currentAmmo+ "MAx"+ AllAmmoleft);
        switch (Type)
        {
            case "AssaltRifle":
                Debug.Log(guntype);

                break;
            case "Pistol":
                Debug.Log(guntype);
                mode1 = new PistolFistMode();
                mode2 = new PistolSeconMode();
                currentMode = mode1;    
                break;
            case "ShotGun":
                Debug.Log(guntype);
                break;

        }
    }
   
    public void OnThrow()
    {
        StartCoroutine(Throwed());
    }
    
    public void SetupGun(IGun m1, IGun m2)
    {
        mode1 = m1;
        mode2 = m2;
        currentMode = mode1; 
    }
    public void ExecuteFire()
    {
        if (Time.time < FirerateCount) return;
      
        if (currentAmmo <= 0)
        {
            ReloadFuc();
            return;
        }
       
        currentMode.shoot(GunPoint);
        FirerateCount = Time.time + guntype.FireRate;
        currentAmmo--;
        if (currentAmmo <= 0 && AllAmmoleft > 0)
        {
            ReloadFuc();
        }
        else
        {
            
            OnAmmoChanged?.Invoke();
        }
        Debug.Log("Shoot " +currentAmmo+ "Type "+ guntype);
    }
    public void SwitchMode()
    {
        currentMode = (currentMode == mode1) ? mode2 : mode1;
    }
    public void ReloadFuc()
    {
        int ammoNeeded = guntype.MaxCapacity - currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, AllAmmoleft);
        currentAmmo += ammoToReload;
        AllAmmoleft -= ammoToReload;
        OnAmmoChanged?.Invoke();
        Debug.Log("reload" + AllAmmoleft);
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
}
