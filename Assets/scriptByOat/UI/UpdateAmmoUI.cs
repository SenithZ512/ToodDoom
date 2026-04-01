using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateAmmoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI CurrenAmmo;
    [SerializeField] private TextMeshProUGUI ammomaxleft;
    [SerializeField] private TextMeshProUGUI gunmode;


    private void Start()
    {
        ClearHud();
    }
    public void UpdateText(int ammo,int maxAmmo,string mode)
    {
        CurrenAmmo.text = ammo.ToString();
        ammomaxleft.text = " / "+maxAmmo.ToString();
        gunmode.text = mode;


    }
    public void ClearHud()
    {
        CurrenAmmo.text = "";
        ammomaxleft.text = "";
        gunmode.text = "";
    }
}
