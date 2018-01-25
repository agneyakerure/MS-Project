using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProperties : MonoBehaviour {

    private GameObject dsp;
    public float health = 100f;
    public float damage = 30f;
	// Use this for initialization
	void Start () {
        dsp = GameObject.Find("DSP");
        //float loudness = dsp.GetComponent<DSP>().checkThisOut;
        //health = loudness * 3f;
        //transform.localScale += new Vector3(loudness * 0.01f, loudness * 0.01f, loudness * 0.01f);
    }
	
	// Update is called once per frame
	void Update () {
        
        //if(dsp.GetComponent<DSP>().flag == true)
        {
            
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collision!");
        if (other.gameObject.tag == "Head")
        {
            Debug.Log("Collieded with Player");
            //Destroy(this.gameObject);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().RemoveHealth(damage);
            //Destroy(this.gameObject);
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
}
