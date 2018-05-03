using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HealthBar : MonoBehaviour {

    public Image currentHealthBar;

    private float hitPoint = 150;
    private float maxHitpoint = 150;
	// Use this for initialization
	void Start () {
        FindObjectOfType<PlayerHealth>().OnHealthPctChanged += HealthIndicator_OnHealthPctChanged;
    }

    private void HealthIndicator_OnHealthPctChanged(float pct)
    {
        currentHealthBar.rectTransform.localScale = new Vector3(pct, 1, 1);
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void updateHealthBar()
    {

    }
}
