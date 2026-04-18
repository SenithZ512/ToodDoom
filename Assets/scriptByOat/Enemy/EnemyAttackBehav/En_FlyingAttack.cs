using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class En_FlyingAttack : MonoBehaviour, IAttackBehaviour
{
    private float yVelocity = 0.0f; 
    public float smoothTime = 0.3f;
    public float TargetHeight = 5f;
    public void Attack(EnemyStateManager state)
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, TargetHeight + 5f))
        {
            state.rb.isKinematic = true;
            float currentHeight = hit.distance;
            float error = TargetHeight - currentHeight;
            state.gameObject.transform.LookAt(state.player.position);
            if (Mathf.Abs(error) > 0.1f)
            {
                state.rb.isKinematic = false;
                float verticalVel = error * 2f;
                verticalVel = Mathf.Clamp(verticalVel, -3f, 3f);

                state.rb.velocity = new Vector3(state.rb.velocity.x, verticalVel, state.rb.velocity.z);
            }
            else
            {
                state.rb.isKinematic = true;
            }
        }
    }
}

