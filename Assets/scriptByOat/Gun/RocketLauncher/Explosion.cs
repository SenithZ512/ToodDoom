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

        Debug.Log("booom");
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in colliders)
        {
           
            if (hit.TryGetComponent<IElement>(out IElement element))
            {
             
                DamageVisitor dmgVisitor = new DamageVisitor(explosionDamage);
                element.Accept(dmgVisitor);
            }

          
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius); // วาดแค่เส้นโครง (Wireframe)
    }
    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(Dissapear());
    }
    
    private IEnumerator Dissapear()
    {
        Explode();
        yield return new WaitForSeconds(TimeToDissaear);
        gameObject.SetActive(false);
    }
}
