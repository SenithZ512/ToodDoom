using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_Death : EnemyBaseState
{
    public override void OnEnterState(EnemyStateManager state)
    {
       state.rb.isKinematic = false;
       state.agent.enabled = false;
      
       
        state.StartCoroutine(dissaper(state));
    }

    public override void OnExitState(EnemyStateManager state)
    {
       
    }

    public override void OnUpdateState(EnemyStateManager state)
    {
       
    }
    private IEnumerator dissaper(EnemyStateManager state)
    {
        yield return new WaitForSeconds(5f);
        state.gameObject.SetActive(false);


    }
}
