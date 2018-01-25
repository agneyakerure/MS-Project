using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayFreq : MonoBehaviour {

    public Text freqText;
    public float freq = 0.0f;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        freq = GameObject.FindObjectOfType<DSP>().fundaFreq;
        freqText.text =  freq.ToString() + " Hz";
    }
}
