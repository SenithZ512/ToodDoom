using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateAmmoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI CurrenAmmo;
    [SerializeField] private TextMeshProUGUI ammomaxleft;
    [SerializeField] private TextMeshProUGUI gunmode;
    [SerializeField] private EquimentSlot equid;


    private void Start()
    {
        //equid=GetComponent<EquimentSlot>();
        ClearHud();
    }
    private void OnEnable()
    {
        GameEvent.UpdateAmmo += UpdateTextV2;
    }
    private void OnDisable()
    {
        GameEvent.UpdateAmmo -= UpdateTextV2;
    }
  
    public void UpdateTextV2()
    {
        if (equid.CurrentHolding == null) return;
        Gun _gun = equid.CurrentHolding.GetComponent<Gun>();
        CurrenAmmo.text = _gun.currentAmmo.ToString();
        ammomaxleft.text = " / " +_gun.AllAmmoleft.ToString();


        gunmode.text = _gun.currentMode.GetType().Name;
    }
    public void ClearHud()
    {
        CurrenAmmo.text = "";
        ammomaxleft.text = "";
        gunmode.text = "";
    }
}
