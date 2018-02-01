using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[RequireComponent(typeof(AudioSource))]
public class DSP : MonoBehaviour
{
    private string[] keyWords;
    private KeywordRecognizer recog;
    public float sensitivity = 100.0f;
    public float frequency = 0.0f;
    public int samplerate = 44100;
    public float[] data = new float[256];
    public int fireCount = 0;
    public int catCount = 0;
    public int dogCount = 0;
    public float loudness;
    public float fundaFreq = 0;
    //public float maxVal = 0;
    public float enemySize = 0;
    public bool flag = false;
    public float checkThisOut = 0.0f;
    public float checkThisPosition = 0.0f;
    public float position = 0.0f;
    public int pair = 0;
    public GameObject target;
    public GameObject target2;

    public List<float> testArr = new List<float>();
    public List<float> positionArr = new List<float>();

    GameObject mic;
    GameObject oscRec;
    GameObject enemy;

    float oldRange = 27.0f;
    float newRange = 10.0f;
    float oldMax = 1037.0f;
    float oldMin = 1010.0f;
    float newMax = 25.0f;
    float newMin = 15.0f;
    float newValue;
    public AudioSource aud1;

    void Start()
    {
        
        keyWords = new string[3];
        keyWords[0] = "Strike";
        keyWords[1] = "Water";
        keyWords[2] = "Dog";
        recog = new KeywordRecognizer(keyWords);
        recog.OnPhraseRecognized += OnPhraseRecognized;
        recog.Start();
        foreach(string device in Microphone.devices)
        {
            Debug.Log(Microphone.devices[0]);
        }
        aud1 = GetComponent<AudioSource>();
        aud1.clip = Microphone.Start(Microphone.devices[0], true, 1, 44100);
        aud1.loop = true; // Set the AudioClip to loop
        aud1.mute = true; // Mute the sound, we don't want the player to hear it

        mic = GameObject.Find("MicControllerC");
        oscRec = GameObject.Find("DSP");
        enemy = GameObject.FindGameObjectWithTag("Enemy");

        //aud1.Play(); // Play the audio source!
    }

    void Update()
    {
        loudness = mic.GetComponent<MicControlC>().loudness;
        fundaFreq = oscRec.GetComponent<ReceivePosition>().pitch;
        position = oscRec.GetComponent<ReceivePosition>().position;
        pair = oscRec.GetComponent<ReceivePosition>().micPairNumber;    //use this for three pairs of microphones for more tracking


        if ((position > 2500) || (position < 1000))
        {
            position = 1023;
        }
        testArr.Add(loudness);
        positionArr.Add(position);
        //maxVal = testArr.Max();
        float last = testArr[testArr.Count - 1];
        //float secondLast = testArr[testArr.Count - 2];
        //float diff = Math.Abs(last - secondLast);
        if(last > 3)
        {
            flag = true;

            float[] loudnessArr = { testArr[testArr.Count - 1], testArr[testArr.Count - 2], testArr[testArr.Count - 3], testArr[testArr.Count - 4], testArr[testArr.Count - 5], testArr[testArr.Count - 6], testArr[testArr.Count - 7], testArr[testArr.Count - 8], testArr[testArr.Count - 9], testArr[testArr.Count - 10], testArr[testArr.Count - 11], testArr[testArr.Count - 12], testArr[testArr.Count - 13], testArr[testArr.Count - 14], testArr[testArr.Count - 15], testArr[testArr.Count - 16], testArr[testArr.Count - 17], testArr[testArr.Count - 18], testArr[testArr.Count - 19], testArr[testArr.Count - 20] };
            checkThisOut = loudnessArr.Max();
            float[] posArr = { positionArr[positionArr.Count - 1], positionArr[positionArr.Count - 2] };
            if(Math.Abs(posArr[0]) > 0)
            {
                checkThisPosition = posArr[0];
            }
            else
            {
                checkThisPosition = posArr[1];
            }
            
        }
        else
        {
            flag = false;
        }


        if (testArr.Count > 250)
        {
            testArr.Clear();
        }
        if(positionArr.Count > 6)
        {
            positionArr.Clear();
        }
       //frequency = GetFundamentalFrequency();
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args )
    {
        enemySize = checkThisOut;
        if(args.text == keyWords[0] /*&& args.confidence > ConfidenceLevel.Low*/)
        {
            Debug.Log("Strike");
            fireCount = fireCount + 1;
            float NewValue = (((checkThisPosition - oldMin) * newRange) / oldRange) + newMin;
            //if(pair == 1)
            {
                Instantiate(target, new Vector3(NewValue, 1.93f, 33.79f), Quaternion.identity);
            }

            //if (pair == 2)
            //{
            //    Instantiate(target, new Vector3(NewValue, 1.93f, 33.79f), Quaternion.identity); //change this z value for pair 1,3
            //}

            //if (pair == 3)
            //{
            //    Instantiate(target, new Vector3(NewValue, 1.93f, 33.79f), Quaternion.identity); //change this z value for pair 2,3
            //}

            target.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            target.GetComponent<EnemyProperties>().health = checkThisOut * 6.0f;
        }

        if (args.text == keyWords[1] /*&& args.confidence > ConfidenceLevel.Low*/)
        {
            Debug.Log("Water");
            catCount = catCount + 1;
            Instantiate(target2, new Vector3(21f, 1f, 30.0f + checkThisPosition * 1.35f), Quaternion.identity);
            target2.transform.localScale = new Vector3(checkThisOut * 0.05f, checkThisOut * 0.05f, checkThisOut * 0.05f);
            target2.GetComponent<EnemyProperties>().health = checkThisOut * 6.0f;
        }

        if (args.text == keyWords[2] /*&& args.confidence > ConfidenceLevel.Low*/)
        {
            Debug.Log("Dog");
            dogCount = dogCount + 1;
        }
    }
}