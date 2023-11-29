using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioControl : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioClip referenceAudioClip;

    public Transform leftHand;
    public Transform rightHand;

    public float pitchRange = 2.0f;
    public float lowPassCutoff = 5000.0f;
    public float spatialBlendRange = 1.0f;

    private AudioLowPassFilter lowPassFilter;

    // Start is called before the first frame update
    void Start()
    {
        audioSource.clip = referenceAudioClip;
        lowPassFilter = audioSource.gameObject.AddComponent<AudioLowPassFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 leftHandPosition = leftHand.position;
        Vector3 rightHandPosition = rightHand.position;

        AdjustAudioProperties(leftHandPosition, rightHandPosition);
    }

    void AdjustAudioProperties(Vector3 leftPosition, Vector3 rightPosition)
    {
        float zDifference = Mathf.Abs(leftPosition.z - rightPosition.z);
        float xDifference = Mathf.Abs(leftPosition.x - rightPosition.x);
        float yDifference = Mathf.Abs(leftPosition.y - rightPosition.y);

        // Adjust pitch based on the z-coordinate difference between hands
        float pitch = MapCoordinateDifferenceToFrequency(zDifference);

        // Adjust spatial blend based on the x-coordinate difference between hands
        float spatialBlend = MapCoordinateDifferenceToSpatialBlend(xDifference);

        // Adjust low pass based on the y-coordinate difference between hands
        float lowPassCutoffFrequency = MapCoordinateDifferenceToLowPassCutoff(yDifference);

        audioSource.pitch = pitch;
        lowPassFilter.cutoffFrequency = lowPassCutoffFrequency;
        audioSource.spatialBlend = spatialBlend;
    }

    float MapCoordinateDifferenceToLowPassCutoff(float difference)
    {
        // Adjust low pass cutoff based on the coordinate difference
        // You can adjust this mapping as needed
        return 500.0f + (difference * lowPassCutoff);
    }

    float MapCoordinateDifferenceToFrequency(float difference)
    {
        // Adjust pitch based on the coordinate difference
        // You can adjust this mapping as needed
        return 1.0f + (difference * pitchRange);
    }

    float MapCoordinateDifferenceToSpatialBlend(float difference)
    {
        // Adjust spatial blend based on the coordinate difference
        // You can adjust this mapping as needed
        return Mathf.Clamp01(difference / spatialBlendRange);
    }
}