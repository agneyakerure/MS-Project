using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    private bool isAttached = false;

    private bool isFired = false;

    public float damage = 75f;

    void OnTriggerStay()
    {
        AttachArrow();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Bow")
        {
            AttachArrow();
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<EnemyProperties>().RemoveHealth(damage);
            //Destroy(collision.gameObject);
        }
    }

    void Update()
    {
        if (isFired && transform.GetComponent<Rigidbody>().velocity.magnitude > 5f)
        {
            transform.LookAt(transform.position + transform.GetComponent<Rigidbody>().velocity);
        }
    }

    public void Fired()
    {
        isFired = true;
    }

    private void AttachArrow()
    {
        var device = SteamVR_Controller.Input((int)ArrowManager.Instance.trackedObj.index);
        if (!isAttached && device.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
        {
            ArrowManager.Instance.AttachBowToArrow();
            isAttached = true;
        }
    }


}
