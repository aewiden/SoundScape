using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectWarp : MonoBehaviour
{
    public AudioSource audioSource;
    public float scaleMultiplier = 1.0f;
    public float minScale = 0.5f;
    public float maxScale = 2.0f;
    public float pitchOscillationRange = 0.5f;

    private Vector3 initialScale;

    // Start is called before the first frame update
    void Start()
    {
        // Store the initial scale of the object
        initialScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (audioSource.isPlaying)
        {
            // Get the audio pitch
            float pitch = audioSource.pitch;

            // Adjust the scale based on the pitch and pitch range
            float newScale = MapPitchToScale(pitch);

            // Apply the new scale to the object, using the initial scale as a reference
            transform.localScale = initialScale * newScale;
        }
    }

    float MapPitchToScale(float pitch)
    {
        // Adjust the scale based on the pitch and pitch range
        float oscillation = Mathf.Sin(Time.time * Mathf.PI * pitchOscillationRange);
        float mappedScale = pitch * scaleMultiplier + oscillation;

        // Clamp the scale to a specific range
        return Mathf.Clamp(mappedScale, minScale, maxScale);
    }
}
