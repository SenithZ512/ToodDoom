using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeAmmo : MonoBehaviour, IAmmo
{
    [SerializeField] private AmmoTypeSO _ammoType;
    [SerializeField] private int _ammoAmount;
    public AmmoTypeSO AmmoType => _ammoType;

    public int AmmoAmount { get => _ammoAmount; set => _ammoAmount = value; }
    private void OnTriggerEnter(Collider other)
    {
        TakeTheAmmo();
    }

    public void TakeTheAmmo()
    {
      
    }
}
