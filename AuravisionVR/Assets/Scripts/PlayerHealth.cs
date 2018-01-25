using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

    [SerializeField]
    public float currentHealth;
    private float maxHealth = 300;

    public event Action<float> OnHealthPctChanged = delegate { };

    private void Awake()
    {
        currentHealth = maxHealth;

    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void RemoveHealth(float amount)
    {
        currentHealth = currentHealth - amount;
        if(currentHealth < 0)
        {
            currentHealth = 0;
        }
        if(currentHealth == 0)
        {

            Application.Quit();
        }
        float pct = currentHealth / maxHealth;
        OnHealthPctChanged(pct);
        //if (currentHealth <= 0)
        //{
        //    Destroy(gameObject);
        //}
    }
}
