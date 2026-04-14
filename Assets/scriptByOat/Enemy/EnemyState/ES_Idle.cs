using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_Idle : EnemyBaseState
{
    public override void OnEnterState(EnemyStateManager state)
    {
       
    }

    public override void OnExitState(EnemyStateManager state)
    {
        state.agent.enabled = true;
    }

    public override void OnUpdateState(EnemyStateManager state)
    {
       

        float distance = Vector3.Distance(state.transform.position, state.player.position);
        //state.agent.SetDestination(state.player.position);
        if (distance <= state.chaseRange)
        {
            state.SwitchState(state._Chase);
        }
       
    }
}
