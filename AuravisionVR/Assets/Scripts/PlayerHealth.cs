using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

    [SerializeField]
    public static float currentHealth;
    public static float maxHealth = 300;
    public GameObject disableBow;
    public GameObject disableShield1;
    public GameObject disableShield2;
    public GameObject gameOverOverlay;

    GameObject dmgOverlay;
    GameObject deathOverlay;
    //public GameObject disableThisObjectOnShieldUp;

    public event Action<float> OnHealthPctChanged = delegate { };

    GameObject[] shields;

    private void Awake()
    {
        currentHealth = maxHealth;

    }
	// Use this for initialization
	void Start () {
        shields = GameObject.FindGameObjectsWithTag("Shield");
        dmgOverlay = GameObject.FindGameObjectWithTag("DamageOverlay");
        deathOverlay = GameObject.FindGameObjectWithTag("DeathOverlay");
    }
	
	// Update is called once per frame
	void Update () {
        if (PlayerHealth.currentHealth <= 0)
        {
            dmgOverlay.SetActive(true);
            deathOverlay.SetActive(true);
        }
        else if (PlayerHealth.currentHealth <= 50)
        {
            dmgOverlay.SetActive(true);
            deathOverlay.SetActive(false);
        }
        else
        {
            dmgOverlay.SetActive(false);
            deathOverlay.SetActive(false);
        }
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
            Debug.Log("Game Over!");
            disableBow.SetActive(false);
            disableShield1.SetActive(false);
            disableShield2.SetActive(false);
            shields[0].SetActive(false);
            shields[1].SetActive(false);
            gameOverOverlay.SetActive(true);
        }
        float pct = currentHealth / maxHealth;
        OnHealthPctChanged(pct);
        //if (currentHealth <= 0)
        //{
        //    Destroy(gameObject);
        //}
    }
}
