using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class EquimentSlot : MonoBehaviour,IThrow
{
    public List<GameObject> gunList = new List<GameObject>();
    public GameObject HoldingPoint;
    public GameObject CurrentHolding;
    [SerializeField] private float ThrowForce = 5f;
    [SerializeField] private UpdateAmmoUI ammoUI;
    private int currentindex;
    private int previousIndex;
    public void AddGun(GameObject newGun)
    {
        if (gunList.Count < 10) 
        {
            gunList.Add(newGun);
            Gun gunScript = newGun.GetComponent<Gun>();
            gunScript.OnAmmoChanged = () => {
                UpdateUI(gunScript);
            };
            if (HoldingPoint)
                newGun.SetActive(false);
        }
       
    }
    private void Update()
    {
        
        for (int i = 0; i < gunList.Count && i < 9; i++)
        {
            
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                if (currentindex != i)
                {
                    previousIndex = currentindex;
                }
                OnHoldGun(i);
                
                if (CurrentHolding != null) UpdateUI(CurrentHolding.GetComponent<Gun>());
                break; 
            }
        }
        if(CurrentHolding!= null)
        {
            Gun currentGun = CurrentHolding.GetComponent<Gun>();
          
            if (Input.GetKeyDown(KeyCode.G))
            {
                OnThrow();
            }
            if (Input.GetMouseButton(0))
            {
                if (currentGun.AllAmmoleft <= 0 && currentGun.currentAmmo <= 0)
                {
                    currentGun.OnAmmoChanged?.Invoke();
                    SwitchToNextAvailableGun();
                    return;
                }
                currentGun.GetComponent<Gun>().ExecuteFire();
               
            }
            if (Input.GetButtonDown("Reload"))
            {
                currentGun.GetComponent<Gun>().ReloadFuc();
                
            }
            if (Input.GetButtonDown("Swamp"))
            {
                SwapToPreviousGun();
            }
        }
       

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Gun>(out Gun _gun))
        {   
            
            _gun.gameObject.transform.position = HoldingPoint.transform.position;
            _gun.gameObject.transform.localRotation = HoldingPoint.transform.localRotation;
            _gun.gameObject.transform.SetParent(HoldingPoint.transform);
            _gun.GetComponent<Collider>().enabled = false;
            
            _gun.GetComponent<Rigidbody>().isKinematic = true;
            AddGun(_gun.gameObject);
        }
    }
    public void SwapToPreviousGun()
    {
      
        if (gunList.Count > 1 && previousIndex < gunList.Count)
        {
            int tempIndex = currentindex; 

            OnHoldGun(previousIndex);

            previousIndex = tempIndex; 

            if (CurrentHolding != null) UpdateUI(CurrentHolding.GetComponent<Gun>());
        }
    }
    private void OnHoldGun(int index)

    {
        if (index < 0 || index >= gunList.Count) return;

        Debug.Log(CurrentHolding);
        for (int i = 0; i < gunList.Count; i++)
    {
            if (i == index)
            {
                bool currentState = !gunList[i].gameObject.activeSelf;
                gunList[i].gameObject.SetActive(currentState);
                if (currentState)
                {
                   
                    CurrentHolding = gunList[index].gameObject;
                    currentindex = i;
                    UpdateUI(gunList[i].GetComponent<Gun>());
                }
                else
                {
                    CurrentHolding = null;
                    ammoUI.ClearHud();
                }
                
               
            }
            else
            {
              ammoUI.ClearHud();
                gunList[i].gameObject.SetActive(false);
            }
        }

    }
    private void SwitchToNextAvailableGun()
    {
        if (gunList.Count <= 1)
        {
           
            if (CurrentHolding != null) CurrentHolding.SetActive(false);
            return;
        }

       
        for (int i = 0; i < gunList.Count; i++)
        {
            
            int nextIndex = (currentindex + 1 + i) % gunList.Count;
            Gun nextGun = gunList[nextIndex].GetComponent<Gun>();

            if (nextGun.currentAmmo > 0 || nextGun.AllAmmoleft > 0)
            {
                OnHoldGun(nextIndex);
               
                return; 
            }
        }

        CurrentHolding.SetActive(false);
    }
    public void OnThrow()
    {
        GameObject thrownGun = CurrentHolding;
        CurrentHolding.transform.SetParent(null);
        CurrentHolding.GetComponent<Gun>().OnThrow();
        CurrentHolding.GetComponent<Rigidbody>().AddForce(HoldingPoint.transform.forward * ThrowForce, ForceMode.Impulse);
        CurrentHolding.GetComponent<Collider>().enabled=true;
        gunList.Remove(CurrentHolding);
        CurrentHolding = null;
        ammoUI.ClearHud();
    }
    private void UpdateUI(Gun gun)
    {
        if (ammoUI != null)
        {
            ammoUI.UpdateText(gun.currentAmmo, gun.AllAmmoleft,gun.currentMode.ModeName);
        }
    }
}
