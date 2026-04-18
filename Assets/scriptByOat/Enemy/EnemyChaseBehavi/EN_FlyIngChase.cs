using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EN_FlyIngChase : MonoBehaviour, IEnemyChase
{
    public float moveSpeed = 5f;
    public float TargetHeight = 10f;
    public float hoverForce = 15f;
    Ray ray;
    RaycastHit hit;
  
    public void Onchase(EnemyStateManager state)
    {
      
        state.agent.enabled = false;
        Vector3 playerPos = new Vector3(state.player.position.x, transform.position.y, state.player.position.z);
        Vector3 moveDirection = (playerPos - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, playerPos);
        if (distanceToPlayer > 0.5f)
        {
            transform.LookAt(playerPos);
            
            state.rb.velocity = new Vector3(moveDirection.x * moveSpeed, state.rb.velocity.y, moveDirection.z * moveSpeed);
        }
        else
        {
            
            state.rb.velocity = new Vector3(0, state.rb.velocity.y, 0);
        }

        ray = new Ray(transform.position, -transform.up);
        if (Physics.Raycast(ray, out hit, TargetHeight))
        {
            float currentDistance = Vector3.Distance(transform.position, hit.point);

            if (currentDistance < TargetHeight)
            {
                float forceMagnitude = (TargetHeight - currentDistance) * hoverForce;
                state.rb.AddForce(Vector3.up * forceMagnitude, ForceMode.Acceleration);
                if (state.rb.velocity.y > 5f)
                {
                    state.rb.velocity = new Vector3(state.rb.velocity.x, 5f, state.rb.velocity.z);
                }
            }
            else if (currentDistance > TargetHeight + 0.5f)
            {

                state.rb.AddForce(Vector3.down * hoverForce * 0.5f, ForceMode.Acceleration);
            }
        }
    }
}
