using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldManager : MonoBehaviour {

    public SteamVR_TrackedObject trackedObj;
    public GameObject disableThisObjectOnShieldUp;
    public GameObject shield;

    public bool isShieldUp;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        UseShield();
    }

    private void UseShield()
    {
        var device = SteamVR_Controller.Input((int)trackedObj.index);
        if (device.GetPress(SteamVR_Controller.ButtonMask.Grip))
        {
            isShieldUp = true;
            disableThisObjectOnShieldUp.SetActive(false);
            shield.SetActive(true);
        }
        else
        {
            isShieldUp = false;
            disableThisObjectOnShieldUp.SetActive(true);
            shield.SetActive(false);
        }
        //Debug.Log(isShieldUp);
    }
}
