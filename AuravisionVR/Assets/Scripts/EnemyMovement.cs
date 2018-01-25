using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {

    private Transform target;
    private float speed = 2.5f;
    private Vector3 speedRot = Vector3.right * 50f;
    float initialY;
    GameObject player;
    public float loudness = 0.0f;
    public float freq = 0.0f;

    float oldRange = 580f;
    float newRange = 3.6f;
    float oldMax = 600f;
    float oldMin = 20f;
    float newMax = 4f;
    float newMin = 0.4f;

    bool isLoud = false;

    // Use this for initialization
    void Start () {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        initialY = transform.position.y;
        player = GameObject.FindGameObjectWithTag("Player");

	}
	
	// Update is called once per frame
	void Update () {
        loudness = GameObject.FindObjectOfType<DSP>().loudness;
        freq = GameObject.FindObjectOfType<DSP>().fundaFreq;
        if(freq > 120)
        {
            freq = 120;
        }
        
        if(loudness > 2.5f)
        {
            isLoud = true;
        }
        else
        {
            isLoud = false;
        }
        
        float dist = Vector3.Distance(player.transform.position, transform.position);
        if(transform.position.y < 7 && dist > 1.5f) //
        {
            float change = (((freq - oldMin) * newRange) / oldRange) + newMin;
            if(isLoud)
            {
                
                Debug.Log(change);
                gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, initialY + change, gameObject.transform.localPosition.z);
            }
            //else
            //{
            //    gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, initialY, gameObject.transform.localPosition.z);
            //}
        }
        


        transform.Rotate(speedRot * Time.deltaTime);
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
	}
}
