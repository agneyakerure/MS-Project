using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ReceivePosition : MonoBehaviour {
    
   	public OSC osc;
    float maxIndex;
    public List<float> positionList = new List<float>();
    public float position;
    public float pitch;

    // Use this for initialization
    void Start () {
        osc.SetAddressHandler("/juce/channel1", OnReceivePosition);
        osc.SetAddressHandler("/juce/channel2", OnReceivePitch);
        //osc.SetAddressHandler("/test1", OnReceiveNumber);
    }

    // Update is called once per frame
    void Update ()
    {

    }

    void OnReceivePosition(OscMessage message)
    {
        float x = message.GetFloat(0);
        position = x;
        //Debug.Log(x);
    }

    void OnReceivePitch(OscMessage message)
    {
        float x = message.GetFloat(0);
        pitch = x;

    }
}
