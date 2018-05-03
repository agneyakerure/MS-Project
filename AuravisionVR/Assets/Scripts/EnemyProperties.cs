using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProperties : MonoBehaviour {

    private GameObject dsp;
    public float health = 100f;
    public float damage = 30f;

    public GameObject explosionEffect;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collision!");
        if (other.gameObject.tag == "Head")
        {
            //Debug.Log("Collieded with Player");
            //Destroy(this.gameObject);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().RemoveHealth(damage);
            Explode();
            Destroy(this.gameObject);
        }
        if (other.gameObject.tag == "Shield")
        {
            //Debug.Log("Collieded with Shield");
            //Destroy(this.gameObject);
            GameObject.FindGameObjectWithTag("Shield").GetComponent<ShieldHealth>().RemoveHealth(damage);
            Explode();
            Destroy(this.gameObject);
        }
    }

    public void RemoveHealth(float amount)
    {
        health = health - amount;
        if(health <= 0)
        {
            Destroy(gameObject);
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
