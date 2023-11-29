using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioControl1 : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioClip referenceAudioClip;

    public Transform leftHand;
    public Transform rightHand;

    public float pitchRange = 6.0f;
    public float volumeRange = 6.0f;

    // Start is called before the first frame update
    void Start()
    {
        audioSource.clip = referenceAudioClip;
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
        float leftHandPitch = MapHandPositionToFrequency(leftPosition);
        float rightHandPitch = MapHandPositionToFrequency(rightPosition);

        float leftHandVolume = MapHandPositionToVolume(leftPosition);
        float rightHandVolume = MapHandPositionToVolume(rightPosition);

        audioSource.pitch = (leftHandPitch + rightHandPitch) / 2.0f;
        audioSource.volume = (leftHandVolume + rightHandVolume) / 2.0f;

    }

    float MapHandPositionToFrequency(Vector3 handPosition)
    {
        float minX = -0.2f;
        float maxX = 0.2f;
        float normalizedX = Mathf.Clamp01((handPosition.x - minX) / (maxX - minX));
        float mappedPitch = 1.0f + (normalizedX * pitchRange);

        return mappedPitch;
    }

    float MapHandPositionToVolume(Vector3 handPosition)
    {
        float minX = -0.2f;
        float maxX = 0.2f;
        float normalizedX = Mathf.Clamp01((handPosition.x - minX) / (maxX - minX));
        float mappedVolume = 1.0f + (normalizedX * volumeRange);

        return mappedVolume;
    }
}
