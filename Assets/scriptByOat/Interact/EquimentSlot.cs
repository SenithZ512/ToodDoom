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
    public HeldStatus held;


    public bool isCritHundredActive = false;
    private void Start()
    {
        held = GetComponent<HeldStatus>();
    }
    public void AddGun(GameObject newGun)
    {
        if (gunList.Count < 10) 
        {
            gunList.Add(newGun);
            Gun gunScript = newGun.GetComponent<Gun>();

            GameEvent.UpdateAmmo?.Invoke();
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
               
                GameEvent.UpdateAmmo?.Invoke();
                break; 
            }
        }
        if(CurrentHolding!= null)
        {
            Gun currentGun = CurrentHolding.GetComponent<Gun>();
          
            if (Input.GetKeyDown(KeyCode.G))
            {
                OnThrow();
                GameEvent.UpdateAmmo?.Invoke();
            }
            if (Input.GetMouseButton(0))
            {
                
                if (currentGun.AllAmmoleft <= 0 && currentGun.currentAmmo <= 0)
                {
                    currentGun.currentAmmo = 0;
                    GameEvent.UpdateAmmo?.Invoke();

                    SwitchToNextAvailableGun();
                    return;
                }
                currentGun.GetComponent<Gun>().ExecuteFire();
                GameEvent.UpdateAmmo?.Invoke();
            }
            if (Input.GetButtonDown("Reload"))
            {
                currentGun.GetComponent<Gun>().ReloadFuc();
                GameEvent.UpdateAmmo?.Invoke();
                
            }
            if (Input.GetButtonDown("Swamp"))
            {
                SwapToPreviousGun();
                GameEvent.UpdateAmmo?.Invoke();
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
        if(collision.gameObject.TryGetComponent<IEffectPickUp> (out IEffectPickUp effectPickUp))
        {
            effectPickUp.Onpickup(this);
        }
    }
    public void SwapToPreviousGun()
    {
      
        if (gunList.Count > 1 && previousIndex < gunList.Count)
        {
            int tempIndex = currentindex; 

            OnHoldGun(previousIndex);

            previousIndex = tempIndex; 

           
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
    public void ActivateCritBuff(float duration)
    {
        StopAllCoroutines(); // ถ้าเก็บซ้ำให้รีเซ็ตเวลาใหม่
        StartCoroutine(CritBuffRoutine(duration));
    }

    private IEnumerator CritBuffRoutine(float duration)
    {
        isCritHundredActive = true;
        Debug.Log("CRIT BUFF ACTIVE!");
        yield return new WaitForSeconds(duration);
        isCritHundredActive = false;
        Debug.Log("CRIT BUFF EXPIRED");
    }
}
