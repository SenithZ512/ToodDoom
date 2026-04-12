using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STG_CritHundred : MonoBehaviour, IEffectPickUp
{
    [SerializeField] private float duration = 20f;
    public void Onpickup(EquimentSlot equid)
    {
        if (equid != null)
        {
            equid.ActivateCritBuff(duration);
            GameEvent.CriticalActive?.Invoke(duration);
            Destroy(gameObject);
        }
    }
}
