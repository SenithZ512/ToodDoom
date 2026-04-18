using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastPlayer : MonoBehaviour
{
    public float Distance = 10f;
    Ray ray;
    RaycastHit hit;
    public KeyCode Key= KeyCode.E;

  
    private void Update()
    {
        ray = new Ray(transform.position, transform.forward);
        if (Input.GetKeyDown(Key))
        {
            if(Physics.Raycast(ray,out hit, Distance))
            {
                Iinteractable interact = hit.collider.GetComponent<Iinteractable>();
                if (interact !=null)
                {
                    
                    interact.Interact();
                }
               
            }
        }
        Debug.DrawRay(ray.origin, ray.direction*Distance, Color.red);
       
    }
    

}
