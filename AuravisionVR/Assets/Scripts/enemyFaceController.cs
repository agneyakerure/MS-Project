using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyFaceController : MonoBehaviour {

    public Transform target;
    int destroyTime = 5;

    // Use this for initialization
    void Start () {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(target);
        Destroy(gameObject, destroyTime);
    }
}
