using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthIndicator : MonoBehaviour {

    private Material mat;

	// Use this for initialization
	void Start () {
        mat = GetComponent<Renderer>().material;
        FindObjectOfType<PlayerHealth>().OnHealthPctChanged += HealthIndicator_OnHealthPctChanged;
	}

    private void HealthIndicator_OnHealthPctChanged(float pct)
    {
        Debug.Log("Here!!!!!!!!" + pct);
        float amount = pct;
        mat.SetFloat("_Cutoff", amount);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
