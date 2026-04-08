using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour, IpoolObject 
{
    [SerializeField] private GunTypeSo gunTypeSo;
    [SerializeField] private float speed;
    private string Explode = "Explode";
    [SerializeField] private float damage =>gunTypeSo.Damage;
    [SerializeField] private Transform Firepoint;
    private Rigidbody rb;


    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        
    }
    
    public void SetFirepoint(Transform point)
    {
        Firepoint=point;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (gunTypeSo.GunTypename == "RocketLauncher")
        {
            Objectpool.Instance.SpawnFromPool(Explode, transform.position, transform.rotation);
            gameObject.SetActive(false);
        }
        if (collision.gameObject.TryGetComponent<IElement> (out IElement _damage))
        {
            //Objectpool.Instance.SpawnFromPool("Blood", transform.position, transform.rotation);
            DamageVisitor DmgVistit = new DamageVisitor (damage);
            _damage.Accept(DmgVistit);
            gameObject.SetActive(false);
        }
        
        else
        {
            gameObject.SetActive(false);
        }
    }
    public void OnobjectSpawn()
    {
        rb.velocity = transform.forward * speed;
    }
}
