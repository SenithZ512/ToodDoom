using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOndeath : MonoBehaviour, IOndeath
{
    private EnemyStateManager stateManager;
    private void Start()
    {
        stateManager = GetComponent<EnemyStateManager>();
    }
    public void OnDeath()
    {
        stateManager.SwitchState(stateManager._Death);
    }
}
