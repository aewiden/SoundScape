using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Synthesizer : MonoBehaviour
{
    public AudioSource audioSource;
    private float[] samples;
    private float sampleRate = 44100;
    private float frequency = 440.0f;

    private OVRHand leftHand;
    private OVRHand rightHand;

    public float amplitude = 1.0f;
    public float frequencyRange = 1000.0f;


    // Start is called before the first frame update
    void Start()
    {
        samples = new float[(int)(sampleRate * audioSource.clip.length)];

        leftHand = GameObject.Find("CustomHandLeft").GetComponent<OVRHand>();
        rightHand = GameObject.Find("CustomHandRight").GetComponent<OVRHand>();
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 leftHandPostion = leftHand.transform.position;
        Vector3 rightHandPosition = rightHand.transform.position;

        frequency = MapHandPositionToFrequency(leftHandPostion);
        GenerateSineWave();
    }

    float MapHandPositionToFrequency(Vector3 handPosition)
    {
        float minX = -0.2f;
        float maxX = 0.2f;
        float normalizedX = Mathf.Clamp01(handPosition.x - minX) / (maxX - minX);
        float mappedFrequency = frequencyRange * normalizedX;

        return mappedFrequency;
    }

    void GenerateSineWave()
    {
        for (int i = 0; i < samples.Length; i++)
        {
            float t = i / sampleRate;
            samples[i] = amplitude * Mathf.Sin(2 * Mathf.PI * frequency * t);
        }

        audioSource.clip.SetData(samples, 0);
        audioSource.Play();
    }
}
