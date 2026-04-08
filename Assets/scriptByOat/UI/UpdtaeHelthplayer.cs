using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdtaeHelthplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI HealthUi;
    [SerializeField] private TextMeshProUGUI ArmourUi;
    private float health;
    private float armour;
    private HeldStatus held;
   
    private void OnEnable()
    {
        GameEvent.UpdatePLayerStatus += GUIUPDATE;
        GUIUPDATE();
    }
    private void OnDisable()
    {
        GameEvent.UpdatePLayerStatus -= GUIUPDATE;
    }
    private void GUIUPDATE()
    {
        held = GetComponent<HeldStatus>();
        health = held._health;
        armour = held._armor;
        HealthUi.text = "Health:"+health.ToString();
        ArmourUi.text = "Armor:"+armour.ToString();
    }
}
