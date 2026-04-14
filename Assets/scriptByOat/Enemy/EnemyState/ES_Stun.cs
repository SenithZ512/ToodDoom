using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_Stun : EnemyBaseState
{
    private Coroutine stunRoutine;
    public override void OnEnterState(EnemyStateManager state)
    {
        state.agent.enabled = false;
        state.rb.isKinematic = false;
        stunRoutine = state.StartCoroutine(count(state));
    }

    public override void OnExitState(EnemyStateManager state)
    {
        state.agent.enabled = true;
        state.rb.isKinematic = true;
        if (stunRoutine != null)
            state.StopCoroutine(stunRoutine);
    }

    public override void OnUpdateState(EnemyStateManager state)
    {
      
    }

    private IEnumerator count(EnemyStateManager state)
    {
        yield return new WaitForSeconds(2f);
        state.SwitchState(state._Idle);
    }
}
