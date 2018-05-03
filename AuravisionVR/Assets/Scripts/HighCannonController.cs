using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighCannonController : MonoBehaviour {

    public Transform target;
    public float force = 0.5f;
    public GameObject grenadePrefab;
    public float highFreq = 0;

    public float coolDown = 2f;
    float coolDownTimer;

    // Use this for initialization
    void Start () {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        grenadePrefab = Resources.Load("Bomb") as GameObject;
    }
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(target);
        highFreq = DSP.highPitch;
        if (coolDownTimer > 0)
        {
            coolDownTimer -= Time.deltaTime;
        }
        if (coolDownTimer < 0)
        {
            coolDownTimer = 0;
        }

        if (highFreq > 13 && coolDownTimer == 0)
        {
            ThrowGrenade();
        }
    }

    void ThrowGrenade()
    {
        GameObject grenade = Instantiate(grenadePrefab, transform.position + new Vector3(0, 0.5f, 0), transform.rotation); //+ new Vector3(0, 1.5f, 0)
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * force, ForceMode.VelocityChange);
        coolDownTimer = coolDown;
    }
}
