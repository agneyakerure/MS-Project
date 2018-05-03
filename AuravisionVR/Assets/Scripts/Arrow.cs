using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    private bool isAttached = false;

    private bool isFired = false;

    public GameObject explosionEffect;

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
            //Debug.Log("Normal Enemy!");
            collision.gameObject.GetComponent<EnemyProperties>().RemoveHealth(damage);
            Explode();
            //Destroy(collision.gameObject);
        }
        if (collision.gameObject.tag == "FaceEnemy")
        {
            //Debug.Log("HERE!");
            collision.gameObject.GetComponent<faceEnemyProperties>().RemoveHealth(damage);
            Explode();
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

    void Explode()
    {
        //Debug.Log("BOOOM!");
        Instantiate(explosionEffect, transform.position, transform.rotation);

        Destroy(explosionEffect);
        Destroy(gameObject);
    }

}
