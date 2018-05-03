using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour {

    public float delay = 3f;
    float countdown;
    bool hasExploded = false;
    public GameObject explosionEffect;

	// Use this for initialization
	void Start () {
        countdown = delay;
	}
	
	// Update is called once per frame
	void Update () {
        countdown -= Time.deltaTime;
        if(countdown <= 0 && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collision!");
        if (other.gameObject.tag == "Head")
        {
            Explode();
        }
        if (other.gameObject.tag == "Shield")
        {
            //Debug.Log("Collieded with Shield");
            //Destroy(this.gameObject);
            GameObject.FindGameObjectWithTag("Shield").GetComponent<ShieldHealth>().RemoveHealth(30);
            Destroy(this.gameObject);
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
