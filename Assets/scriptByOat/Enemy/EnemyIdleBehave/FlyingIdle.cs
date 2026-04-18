using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlyingIdle : MonoBehaviour, IEnemyIdleBehave
{
    public float TargetHeight = 10f;
    public float hoverForce = 15f;
    Ray ray;
    RaycastHit hit;
    private Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
       
    }
  
    public void OnIdle(EnemyStateManager state)
    {

        if (TryGetComponent<UnityEngine.AI.NavMeshAgent>(out var agent))
        {
            agent.enabled = false;
            rb.velocity = Vector3.zero;
        }
        ray = new Ray(transform.position, -transform.up);
        if (Physics.Raycast(ray, out hit, TargetHeight))
        {
            float currentDistance = Vector3.Distance(transform.position, hit.point);

            if (currentDistance < TargetHeight)
            {
                float forceMagnitude = (TargetHeight - currentDistance) * hoverForce;
                rb.AddForce(Vector3.up * forceMagnitude, ForceMode.Acceleration);
                if (rb.velocity.y > 5f)
                {
                    rb.velocity = new Vector3(rb.velocity.x, 5f, rb.velocity.z);
                }
            }
            else if (currentDistance > TargetHeight + 0.5f)
            {

                rb.AddForce(Vector3.down * hoverForce * 0.5f, ForceMode.Acceleration);
            }

        }
        Debug.DrawRay(ray.origin, ray.direction * TargetHeight, Color.red);
    }
}
