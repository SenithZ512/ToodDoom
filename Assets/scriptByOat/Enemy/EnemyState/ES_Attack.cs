using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ES_Attack : EnemyBaseState
{
    private IAttackBehaviour[] _attackBehaviour;
    public override void OnEnterState(EnemyStateManager state)
    {
      
        _attackBehaviour = state.gameObject.GetComponents<IAttackBehaviour>();
    
    }

    public override void OnExitState(EnemyStateManager state)
    {
        if (_attackBehaviour != null)
        {
            state.rb.isKinematic = false;
        }
       
    }

    public override void OnUpdateState(EnemyStateManager state)
    {
        float distance = Vector3.Distance(state.transform.position, state.player.position);
        Vector3 direction = (state.player.position - state.transform.position).normalized;
        direction.y = 0; 

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
           
            state.transform.rotation = Quaternion.Slerp(state.transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        if (distance > state.attackRange&& distance <= state.chaseRange)
        {
            state.SwitchState(state._Chase);
            return;
        }
        if (_attackBehaviour != null)
        {
            foreach (var behaviour in _attackBehaviour)
            {
                behaviour.Attack(state);
            }
        }

    }
}
