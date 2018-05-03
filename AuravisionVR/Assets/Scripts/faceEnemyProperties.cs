using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class faceEnemyProperties : MonoBehaviour
{

    private GameObject dsp;
    public float health = 100f;
    public float damage = 30f;

    public GameObject explosionEffect;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("HERE 2!");
        //Debug.Log("Collision!");
        if (other.gameObject.tag == "Head")
        {
            //Debug.Log("Collieded with Player");
            //Destroy(this.gameObject);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().RemoveHealth(damage);
            Explode();
            Destroy(this.gameObject);
        }
        if (other.gameObject.tag == "Arrow")
        {
            //Debug.Log("Collieded with Player");
            //Destroy(this.gameObject);
            //Debug.Log("HERE!");
            Explode();
            Destroy(this.gameObject);
        }
    }

    public void RemoveHealth(float amount)
    {
        health = health - amount;
        if (health <= 0)
        {
            DSP.enemyHealth -= 1;
            Destroy(gameObject);
        }
    }

    void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(explosionEffect);
    }
}

