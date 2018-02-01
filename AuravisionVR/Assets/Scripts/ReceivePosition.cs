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
    public int micPairNumber;
    // Use this for initialization
    void Start () {
	   //osc.SetAddressHandler( "/CubeXYZ" , OnReceiveXYZ );
    //   osc.SetAddressHandler("/CubeX", OnReceiveX);
    //   osc.SetAddressHandler("/CubeY", OnReceiveY);
    //   osc.SetAddressHandler("/CubeZ", OnReceiveZ);
        osc.SetAddressHandler("/juce/channel1", OnReceiveTest);
        osc.SetAddressHandler("/juce/channel2", OnReceivePitch);
        osc.SetAddressHandler("/juce/channel3", OnReceiveNumber);
        ///juce/channel1
    }

    // Update is called once per frame
    void Update () {
        positionList.Add(maxIndex);
        position = positionList.Max();
        if (positionList.Count > 10)
        {
            positionList.Clear();
        }
    }

    void OnReceiveTest(OscMessage message)
    {
        float x = message.GetFloat(0);
        if(x < 1000)
        {
            x = 1023;
        }
        maxIndex = x;
        
    }

    void OnReceivePitch(OscMessage message)
    {
        float x = message.GetFloat(0);
        pitch = x;

    }

    void OnReceiveNumber(OscMessage message)
    {
        int x = message.GetInt(0);
        micPairNumber = x;
    }

    //   void OnReceiveXYZ(OscMessage message){
    //	float x = message.GetFloat(0);
    //        float y = message.GetFloat(1);
    //	float z = message.GetFloat(2);

    //	transform.position = new Vector3(x,y,z);
    //}

    //   void OnReceiveX(OscMessage message) {
    //       float x = message.GetFloat(0);

    //       Vector3 position = transform.position;

    //       position.x = x;

    //       transform.position = position;
    //   }

    //   void OnReceiveY(OscMessage message) {
    //       float y = message.GetFloat(0);

    //       Vector3 position = transform.position;

    //       position.y = y;

    //       transform.position = position;
    //   }

    //   void OnReceiveZ(OscMessage message) {
    //       float z = message.GetFloat(0);

    //       Vector3 position = transform.position;

    //       position.z = z;

    //       transform.position = position;
    //   }

}
