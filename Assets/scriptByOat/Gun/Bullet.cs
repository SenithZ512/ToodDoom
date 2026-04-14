using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour, IpoolObject 
{
    [SerializeField] private GunTypeSo gunTypeSo;
    [SerializeField] private float speed;
    private string Explode = "Explode";
    [SerializeField] private float damage;
    [SerializeField] private Transform Firepoint;
    private Rigidbody rb;


    private bool _isCrit;
    public void Setup(float dmg, bool crit)
    {
        damage = dmg;
        _isCrit = crit;
    }

   
   
   
    private void OnTriggerEnter(Collider collision)
    {
        if (gunTypeSo.GunTypename == "RocketLauncher")
        {
            Objectpool.Instance.SpawnFromPool(Explode, transform.position, transform.rotation);
            gameObject.SetActive(false);
            return;
        }
        /*  float finalDamage = damage;
          if (gameObject.CompareTag("Bullet") && Critmanager.PlayerCritActive)
          {
              finalDamage = damage +damage * 2f; 
          }*/

        if (collision.gameObject.TryGetComponent<IElement>(out IElement _damage))
        {
            //Objectpool.Instance.SpawnFromPool("Blood", transform.position, transform.rotation);
            DamageVisitor DmgVistit = new DamageVisitor(damage);
            _damage.Accept(DmgVistit);
            gameObject.SetActive(false);
            if (_isCrit) Debug.Log("BOOM! CRITICAL HIT!");
        }
        gameObject.SetActive(false);
    }
    public void OnobjectSpawn()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        StartCoroutine(dissaper());
       
    }
    IEnumerator dissaper()
    {
        yield return new WaitForSeconds(10f);
        gameObject.SetActive(false);
    }
}
