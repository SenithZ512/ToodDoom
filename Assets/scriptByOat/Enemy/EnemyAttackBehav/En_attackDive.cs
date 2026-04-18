using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class En_attackDive : MonoBehaviour,IAttackBehaviour
{
    public float diveSpeed = 20f;
    public float overshootDistance = 5f; 
    private Vector3 targetPoint;
    private bool isDiving = false;

    public void Attack(EnemyStateManager state)
    {
        if (!isDiving)
        {
            
            Vector3 direction = (state.player.position - transform.position).normalized;
            targetPoint = state.player.position + (direction * overshootDistance);
            isDiving = true;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPoint, diveSpeed * Time.deltaTime);
        transform.LookAt(targetPoint); 

      
        if (Vector3.Distance(transform.position, targetPoint) < 0.5f)
        {
            isDiving = false;
          
            state.SwitchState(state._Idle);
        }
    }


}
