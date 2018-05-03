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
    //public float[] data = new float[256];

    public float[] spectrumDataFromMicControlC = new float[256];

    public static int enemyHealth = 20;

    public int fireCount = 0;

    public float loudness;
    public float fundaFreq = 0;

    public float enemySize = 0;
    public bool flag = false;
    public float sizeDependentOnLoudness = 0.0f;
    public float checkThisPosition = 0.0f;
    public float position = 0.0f;

    public GameObject target;
    public GameObject lowCannon;
    public GameObject highCannon;

    public GameObject enemyFace;

    int lowCannonCount = 0;
    int highCannonCount = 0;
    public static float lowPitch = 0;
    public static float highPitch = 0;
    public GameObject mainCamera;

    public List<float> testArr = new List<float>();
    public List<float> positionArr = new List<float>();
    public static float[] _freqBand = new float[8];

    public Text enemyHealthText;

    GameObject mic;
    GameObject oscRec;
    GameObject enemy;
    GameObject player;

    GameObject dmgOverlay;
    GameObject deathOverlay;

    GameObject enemydmgOverlay;
    GameObject enemydeathOverlay;
    //GameObject mainCamera;

    float oldRange = 27.0f;
    float newRange = 10.0f;
    float oldMax = 1037.0f;
    float oldMin = 1010.0f;
    float newMax = 25.0f;
    float newMin = 15.0f;
    float newValue;
    public AudioSource aud1;
    float subtractorAngle;

    void Start()
    {

        keyWords = new string[4];
        keyWords[0] = "Strike";
        keyWords[1] = "Thunder";
        keyWords[2] = "Dog";
        keyWords[3] = "Cannon";

        recog = new KeywordRecognizer(keyWords);
        recog.OnPhraseRecognized += OnPhraseRecognized;
        recog.Start();
        foreach (string device in Microphone.devices)
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
        player = GameObject.FindGameObjectWithTag("Player");

        dmgOverlay = GameObject.FindGameObjectWithTag("DamageOverlay");
        deathOverlay = GameObject.FindGameObjectWithTag("DeathOverlay");

        enemydmgOverlay = GameObject.FindGameObjectWithTag("enemyDamageOverlay");
        enemydeathOverlay = GameObject.FindGameObjectWithTag("enemyDeathOverlay");
    }

    void Update()
    {
        loudness = mic.GetComponent<MicControlC>().loudness;
        fundaFreq = mic.GetComponent<MicControlC>().fundaFreq;
        position = oscRec.GetComponent<ReceivePosition>().position;
        spectrumDataFromMicControlC = mic.GetComponent<MicControlC>().spectrumData2;

        enemyHealthText.text = "Enemy Lives: " + enemyHealth;

        //Debug.Log(enemyHealth);
        if (enemyHealth <= 0)
        {
            enemydmgOverlay.SetActive(true);
            enemydeathOverlay.SetActive(true);
        }
        else if (enemyHealth <= 5)
        {
            enemydmgOverlay.SetActive(true);
            enemydeathOverlay.SetActive(false);
        }
        else
        {
            enemydmgOverlay.SetActive(false);
            enemydeathOverlay.SetActive(false);
        }

        lowPitch = (_freqBand[0] * 10) + 1;
        highPitch = (_freqBand[3] * 10) + 1;

        MakeFrequencyBands();

        subtractorAngle = (float)Math.Round(mainCamera.transform.eulerAngles.y / 30) - 30;
        //subtractorAngle = 0;

        testArr.Add(loudness);
        float last = testArr[testArr.Count - 1];
        if (last > 3)
        {
            flag = true;

            float[] loudnessArr = { testArr[testArr.Count - 1], testArr[testArr.Count - 2], testArr[testArr.Count - 3], testArr[testArr.Count - 4], testArr[testArr.Count - 5], testArr[testArr.Count - 6], testArr[testArr.Count - 7], testArr[testArr.Count - 8], testArr[testArr.Count - 9], testArr[testArr.Count - 10], testArr[testArr.Count - 11], testArr[testArr.Count - 12], testArr[testArr.Count - 13], testArr[testArr.Count - 14], testArr[testArr.Count - 15], testArr[testArr.Count - 16], testArr[testArr.Count - 17], testArr[testArr.Count - 18], testArr[testArr.Count - 19], testArr[testArr.Count - 20] };
            sizeDependentOnLoudness = loudnessArr.Max();
        }
        else
        {
            flag = false;
        }


        if (testArr.Count > 250)
        {
            testArr.Clear();
        }
    }

    void MakeFrequencyBands()
    {
        int count = 0;
        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;
            if (i == 7)
            {
                sampleCount += 2;
            }
            for (int j = 0; j < sampleCount; j++)
            {
                average += spectrumDataFromMicControlC[count] * (count + 1);
                count++;
            }

            average /= count;

            _freqBand[i] = average * 10;
        }

    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        float radianOuter = (position - subtractorAngle) * Mathf.Deg2Rad;
        float x1 = Mathf.Cos(radianOuter) * 9.3f;
        float y1 = Mathf.Sin(radianOuter) * 9.3f;

        Vector3 pos1 = new Vector3(y1, 0, x1) + player.transform.position;// + new Vector3(0, 1.33f, 0);

        {
            Instantiate(enemyFace, pos1, Quaternion.identity);
        }

        enemySize = sizeDependentOnLoudness;
        if (args.text == keyWords[0] /*&& args.confidence > ConfidenceLevel.Low*/)
        {
            //Debug.Log("Strike");
            fireCount = fireCount + 1;
            //float degree = position;
            float radian = (position - subtractorAngle) * Mathf.Deg2Rad;
            float x = Mathf.Cos(radian) * 6.5f;
            float y = Mathf.Sin(radian) * 6.5f;
            //Debug.Log("Radians : " + radian);
            //Debug.Log("X : " + x + " Y : " + y);
            //Debug.Log("Player position: " + player.transform.position);
            Vector3 pos = new Vector3(y, 0, x) + player.transform.position;
            //Debug.Log("Pos X: " + pos.x + "Pos Y: " + pos.y);
            
            {
                Instantiate(target, pos, Quaternion.identity);
            }

            target.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            target.GetComponent<EnemyProperties>().health = sizeDependentOnLoudness * 6.0f;
        }

        if (args.text == keyWords[1] /*&& args.confidence > ConfidenceLevel.Low*/)
        {
            //fireCount = fireCount + 1;
            if(lowCannonCount < 1)
            {
                float radian = (position - subtractorAngle) * Mathf.Deg2Rad;
                float x = Mathf.Cos(radian) * 6;
                float y = Mathf.Sin(radian) * 6;

                Vector3 pos = new Vector3(y, 0, x) + player.transform.position;// + new Vector3(0, 1.33f, 0);

                {
                    Instantiate(lowCannon, pos, Quaternion.identity);
                }

                lowCannon.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                lowCannonCount++;
            }
        }

        if (args.text == keyWords[2] /*&& args.confidence > ConfidenceLevel.Low*/)
        {
            //fireCount = fireCount + 1;
            if (highCannonCount < 1)
            {
                float radian = (position - subtractorAngle) * Mathf.Deg2Rad;
                float x = Mathf.Cos(radian) * 6;
                float y = Mathf.Sin(radian) * 6;

                Vector3 pos = new Vector3(y, 0, x) + player.transform.position;// + new Vector3(0, 1.33f, 0);

                {
                    Instantiate(highCannon, pos, Quaternion.identity);
                }

                highCannon.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                highCannonCount++;
            }
        }
    }
}