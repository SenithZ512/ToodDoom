using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float TimeToDissaear = 2f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionDamage = 150f;
    [SerializeField] private float explosionForce = 700f;

  
    public void Explode()
    {
       
       
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in colliders)
        {
            if (hit.gameObject == gameObject)
            {
                hit.enabled = false;
                
               
            }

            if (hit.TryGetComponent<IElement>(out IElement element))
            {
             
                DamageVisitor dmgVisitor = new DamageVisitor(explosionDamage);
                element.Accept(dmgVisitor);
            }

            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                if (rb.gameObject == gameObject)
                {
                    rb.isKinematic = true;
                  
                }
                if(rb.gameObject.TryGetComponent<EnemyStateManager>(out EnemyStateManager enemyStateManager))
                {
                    enemyStateManager.SwitchState(enemyStateManager._Stun);
                }
           
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius,3f);
            }
        }

    }
  

    private void OnCollisionEnter(Collision collision)
    {
        
        Explode();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius); 
    }
    private void OnEnable()
    {
        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        
        if (TryGetComponent<Collider>(out Collider col))
        {
            col.enabled = true;
        }
        StopAllCoroutines();

        StartCoroutine(Dissapear());
    }

    private IEnumerator Dissapear()
    {
       
        yield return new WaitForSeconds(TimeToDissaear);
        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        gameObject.SetActive(false);
    }
}
