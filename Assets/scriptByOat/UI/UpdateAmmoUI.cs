using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateAmmoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI CurrenAmmo;
    [SerializeField] private TextMeshProUGUI ammomaxleft;
   

    private void Start()
    {
        ClearHud();
    }
    public void UpdateText(int ammo,int maxAmmo)
    {
        CurrenAmmo.text = ammo.ToString();
        ammomaxleft.text = " / "+maxAmmo.ToString();
       
    }
    public void ClearHud()
    {
        CurrenAmmo.text = "";
        ammomaxleft.text = "";
    }
}
