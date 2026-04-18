using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeAmmo : MonoBehaviour, IAmmo
{
    [SerializeField] private AmmoTypeSO _ammoType;
    [SerializeField] private int _ammoAmount;
    public AmmoTypeSO AmmoType => _ammoType;

    public int AmmoAmount { get => _ammoAmount; set => _ammoAmount = value; }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<EquimentSlot>(out EquimentSlot equid))
        {
            bool wasAmmoAdded = false;
            foreach (GameObject gunObject in equid.gunList)
            {
                if (gunObject.TryGetComponent<Gun>(out Gun gunScript))
                {
                    if (gunScript.GetAmmoType() == _ammoType)
                    {
                        gunScript.Addammo(_ammoAmount);
                        wasAmmoAdded = true;
                        if(equid.CurrentHolding == gunObject)
                        {
                            GameEvent.UpdateAmmo?.Invoke();
                        }
                    }
                }
            }
            if (wasAmmoAdded) TakeTheAmmo();
        }
    }

    public void TakeTheAmmo()
    {
        Destroy(gameObject);
    }
}
