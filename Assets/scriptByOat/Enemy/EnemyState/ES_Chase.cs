using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_Chase : EnemyBaseState
{
    private IEnemyChase chase;

    public override void OnEnterState(EnemyStateManager state)
    {
        chase = state.gameObject.GetComponent<IEnemyChase>();
    }


    public override void OnExitState(EnemyStateManager state)
    {
        if (state.agent.isOnNavMesh) state.agent.ResetPath();
    }


    public override void OnUpdateState(EnemyStateManager state)
    {
        if(chase != null)
        {
            chase.Onchase(state);
           
        }

        float distance = Vector3.Distance(state.transform.position, state.player.position);
        if (state.agent.enabled && state.agent.isOnNavMesh)
        {
            state.agent.SetDestination(state.player.position);
        }
        if (distance <= state.attackRange)
        {
            state.SwitchState(state._Attack); 
        }
        if (distance > state.chaseRange)
        {
            state.SwitchState(state._Idle);
        }
    }
}
