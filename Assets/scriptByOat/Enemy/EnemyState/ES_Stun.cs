using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_Stun : EnemyBaseState
{
    private bool wasUsingAgent;
    private Coroutine stunRoutine;
    public override void OnEnterState(EnemyStateManager state)
    {
        wasUsingAgent = state.agent.enabled;

        
        state.agent.enabled = false;
        state.rb.isKinematic = false;

        stunRoutine = state.StartCoroutine(count(state));
    }

    public override void OnExitState(EnemyStateManager state)
    {
        state.rb.velocity = Vector3.zero;
        state.rb.angularVelocity = Vector3.zero;
        Vector3 currentForward = state.transform.forward;
        currentForward.y = 0; 
        if (currentForward != Vector3.zero)
        {
            state.transform.rotation = Quaternion.LookRotation(currentForward, Vector3.up);
        }
        if (wasUsingAgent)
        {
            state.agent.enabled = true;
            state.rb.isKinematic = true;
        }
        else
        {
           
            state.rb.isKinematic = false;

        }

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
