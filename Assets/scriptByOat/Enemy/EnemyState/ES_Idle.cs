using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_Idle : EnemyBaseState
{
    private IEnemyIdleBehave idle;
    public override void OnEnterState(EnemyStateManager state)
    {
        idle = state.gameObject.GetComponent<IEnemyIdleBehave>();
    }

    public override void OnExitState(EnemyStateManager state)
    {
        if(idle == null)
        {
            state.agent.enabled = true;
        }
       
    }

    public override void OnUpdateState(EnemyStateManager state)
    {
       if(idle != null)
        {
            idle.OnIdle(state);
        }

        float distance = Vector3.Distance(state.transform.position, state.player.position);
        //state.agent.SetDestination(state.player.position);
        if (distance <= state.chaseRange)
        {
            state.SwitchState(state._Chase);
        }
       
    }
}
