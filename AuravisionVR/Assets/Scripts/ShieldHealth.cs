using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ShieldHealth : MonoBehaviour
{

    [SerializeField]
    public float currentHealth;
    private float maxHealth = 100;
    //public GameObject disableThisObjectOnShieldUp;

    //public event Action<float> OnHealthPctChanged = delegate { };

    //GameObject[] shields;

    private void Awake()
    {
        currentHealth = maxHealth;

    }
    // Use this for initialization
    void Start()
    {
        //shields = GameObject.FindGameObjectsWithTag("Shield");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RemoveHealth(float amount)
    {
        currentHealth = currentHealth - amount;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        if (currentHealth <= 0)
        {
            Debug.Log("Game Over!");
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        //float pct = currentHealth / maxHealth;
        //OnHealthPctChanged(pct);
        //if (currentHealth <= 0)
        //{
            
        //}
    }
}
