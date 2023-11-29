using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public class MusicDataController1 : MonoBehaviour
{
    public AudioSource audioSource;
    public TextAsset jsonFile;
    private MusicData musicData;

    [Header("Beat Objects")]
    public GameObject beatObject;
    public Material defaultMaterial;
    public Material emissiveMaterial;
    public Light beatLight;
    [Tooltip("The time it takes for downbeats to fade back to the default material.")]
    public float fadeDuration = 0.5f;

    [Header("Energy Objects")]
    public GameObject energyObject; // Objects to apply materials to
    public Light highEnergyLight; // Lights to activate for high energy
    public Light lowEnergyLight; // Lights to activate for low energy

    [Space]
    public Material highEnergyMaterial;
    public Material lowEnergyMaterial;

    [Header("Frequency Data Objects")]
    public GameObject bassObject;
    public GameObject midObject;
    public GameObject trebleObject;

    [Space]
    public float yAnimationScale = 1f; // Scale factor for Y animation of frequency data objects
    public float yAnimationScaleBias; // Bias for yAnimationScale

    [Space]
    public float lightIntensityScale = 1f; // Scale factor for light intensity
    public float lightIntensityScaleBias; // Bias for lightIntensityScale

    [Header("Volume Data Objects")]
    public Light spotlight; // Reference to the spotlight
    public float minSpotlightIntensity = 0f; // Minimum intensity of the spotlight
    public float maxSpotlightIntensity = 750f; // Maximum intensity of the spotlight

    [Header("Transient Objects")]
    public GameObject[] transientObjects; // Should be assigned to 5 objects in the Unity Inspector

    [Header("Transient Materials")]
    public Material inactiveMaterial; // Assign the 'inactive' material here
    public Material[] activeMaterials; // Assign 5 'active' materials here, one for each object

    [Header("Transient Animation Control")]
    public float transientMaxScale = 3f; // Maximum allowed scale
    public float scaleDownFactor = 0.1f; // Factor to scale down the objects that are too large
    public float transientAnimationDuration = 1.0f; // Duration of the transient pulse animation
    public float transientFadeDuration = 0.5f; // Duration of the fade out for transient animation

    private float dataInterval; // Time interval between each data point

    private float initialLightIntensity; // Add this variable to store the initial intensity
    private Coroutine fadeCoroutine; // To keep track of the coroutine

    // This will help us not to check each beat again and again
    private int nextBeatIndex = 0;

    // Use these indexes to keep track of high and low energy beats
    private int nextHighEnergyIndex = 0;
    private int nextLowEnergyIndex = 0;

    private float volumeDataInterval; // Time interval between each volume data point
    // private int nextVolumeIndex = 0; // To keep track of the current volume data index

    private int nextTransientIndex = 0;
    private Coroutine[] transientCoroutines; // To keep track of the fade coroutines for each transient object

    private Vector3[] initialScales; // To keep track of the initial scales of objects

    // A threshold for checking events
    private const float TIME_THRESHOLD = 0.05f; // e.g. 50 milliseconds

    void Start()
    {
        LoadJson();
        PlayMusic();
        initialLightIntensity = beatLight.intensity;

        // Assume `songDuration` is the total duration of your song in seconds
        float songDuration = audioSource.clip.length;
        // Calculate the time interval between each data point
        dataInterval = songDuration / musicData.frequency_data.bass.amplitude_values.Count;

        // Initialize the volume data interval
        volumeDataInterval = songDuration / musicData.smoothed_volume.Count;

        transientCoroutines = new Coroutine[transientObjects.Length]; // Initialize the array based on the number of transient objects

        // Initialize the initialScales array to store the original scale of each transient object
        initialScales = new Vector3[transientObjects.Length];
        for (int i = 0; i < transientObjects.Length; i++)
        {
            initialScales[i] = transientObjects[i].transform.localScale;
        }
    }

    public void PlayMusic()
    {
        audioSource.Play();
        // UnityEngine.Debug.Log("Music started!");
        StartCoroutine(MonitorPlayback());
    }

    void LoadJson()
    {
        musicData = JsonUtility.FromJson<MusicData>(jsonFile.text);
    }

    IEnumerator MonitorPlayback()
    {
        float prevTime = -1f; // To keep track of the last update time
        float prevVolumeTime = -1f; // To keep track of the last volume update time

        while (audioSource.isPlaying)
        {
            float currentTime = audioSource.time;
            CheckEvents(currentTime);

            // Update frequency data
            if (currentTime - prevTime >= dataInterval)
            {
                prevTime = currentTime;
                int dataIndex = Mathf.FloorToInt(currentTime / dataInterval);
                UpdateFrequencyData(dataIndex);
            }

            // Update volume data
            if (currentTime - prevVolumeTime >= volumeDataInterval)
            {
                prevVolumeTime = currentTime;
                int volumeIndex = Mathf.FloorToInt(currentTime / volumeDataInterval);
                UpdateVolumeData(volumeIndex);
            }

            yield return null; // Wait for the next frame
        }
    }

    void CheckEvents(float currentTime)
    {
        // Check beats using the index and threshold
        if (nextBeatIndex < musicData.beat_data.beat_times.Count)
        {
            float nextBeatTime = musicData.beat_data.beat_times[nextBeatIndex];

            if (Mathf.Abs(currentTime - nextBeatTime) <= TIME_THRESHOLD)
            {
                TriggerBeatAnimation();
                nextBeatIndex++;  // Move to the next beat
            }
        }

        // Check high energy beats
        if (nextHighEnergyIndex < musicData.energy_data.high_energy_beats.Count)
        {
            float nextHighEnergyTime = musicData.energy_data.high_energy_beats[nextHighEnergyIndex];
            if (Mathf.Abs(currentTime - nextHighEnergyTime) <= TIME_THRESHOLD)
            {
                TriggerHighEnergyState();
                nextHighEnergyIndex++;
            }
        }

        // Check low energy beats
        if (nextLowEnergyIndex < musicData.energy_data.low_energy_beats.Count)
        {
            float nextLowEnergyTime = musicData.energy_data.low_energy_beats[nextLowEnergyIndex];
            if (Mathf.Abs(currentTime - nextLowEnergyTime) <= TIME_THRESHOLD)
            {
                TriggerLowEnergyState();
                nextLowEnergyIndex++;
            }
        }

        // Check transients using the index and threshold
        while (nextTransientIndex < musicData.transients.onset_times.Count)
        {
            float nextTransientTime = musicData.transients.onset_times[nextTransientIndex];

            if (currentTime >= nextTransientTime - TIME_THRESHOLD && currentTime <= nextTransientTime + TIME_THRESHOLD)
            {
                // Trigger transient animation if we're within the threshold window
                int label = musicData.transients.labels[nextTransientIndex];
                TriggerTransientAnimation(label);
                nextTransientIndex++; // Move to the next transient
            }
            else if (currentTime > nextTransientTime + TIME_THRESHOLD)
            {
                // Skip this transient if we've passed the threshold window
                nextTransientIndex++;
            }
            else
            {
                // If we're not yet at the next transient time, exit the loop
                break;
            }
        }
    }

    void TriggerBeatAnimation()
    {
        // Change the cube's material to the emissive material
        beatObject.GetComponent<Renderer>().material = emissiveMaterial;
        beatLight.enabled = true;
        // If a fade is already in progress, stop it before starting a new one
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeOutLightAndMaterial()); // Start the coroutine and store its reference
    }

    IEnumerator FadeOutLightAndMaterial()
    {
        float currentIntensity = beatLight.intensity; // Use the current intensity here

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            // Interpolate the intensity and emissive color based on elapsed time
            float normalizedTime = t / fadeDuration;
            beatLight.intensity = Mathf.Lerp(currentIntensity, 0, normalizedTime);

            // Assuming that the emission color is set to be the same as the light color
            Color currentColor = Color.Lerp(beatLight.color * initialLightIntensity, Color.black, normalizedTime);
            beatObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", currentColor);

            yield return null;
        }

        // Ensure the values are set to their final state
        beatLight.enabled = false;
        beatObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.black);
        ResetMaterial();
        beatLight.intensity = initialLightIntensity; // Reset the intensity after the coroutine ends
    }

    void ResetMaterial()
    {
        // Change the material back to default
        beatObject.GetComponent<Renderer>().material = defaultMaterial;
    }

    void TriggerHighEnergyState()
    {
        // Only transition if the current state is not already high energy
        if (energyObject.GetComponent<Renderer>().material != highEnergyMaterial)
        {
            energyObject.GetComponent<Renderer>().material = highEnergyMaterial;
            highEnergyLight.enabled = true;
            lowEnergyLight.enabled = false;
        }
    }

    void TriggerLowEnergyState()
    {
        // Only transition if the current state is not already low energy
        if (energyObject.GetComponent<Renderer>().material != lowEnergyMaterial)
        {
            energyObject.GetComponent<Renderer>().material = lowEnergyMaterial;
            lowEnergyLight.enabled = true;
            highEnergyLight.enabled = false;
        }
    }

    void UpdateFrequencyData(int index)
    {
        // Guard clause to prevent out-of-range errors
        if (index < 0 || index >= musicData.frequency_data.bass.amplitude_values.Count)
            return;

        // Get the amplitude values for bass, mid, and treble at the current index
        float bassValue = musicData.frequency_data.bass.amplitude_values[index];
        float midValue = musicData.frequency_data.mid.amplitude_values[index];
        float trebleValue = musicData.frequency_data.treble.amplitude_values[index];

        // Apply the amplitude values to your objects, scaling them on the y-axis only
        // The yAnimationScale factor is used to control the scaling magnitude
        // Apply biases to scale factors when adjusting properties
        bassObject.transform.localScale = new Vector3(bassObject.transform.localScale.x, bassValue * yAnimationScale + yAnimationScaleBias, bassObject.transform.localScale.z);
        midObject.transform.localScale = new Vector3(midObject.transform.localScale.x, midValue * yAnimationScale + yAnimationScaleBias, midObject.transform.localScale.z);
        trebleObject.transform.localScale = new Vector3(trebleObject.transform.localScale.x, trebleValue * yAnimationScale + yAnimationScaleBias, trebleObject.transform.localScale.z);

        // Adjust the emission intensity and the light brightness based on amplitude values
        // Scale factors are applied to adjust their intensity
        AdjustEmissionAndLighting(bassObject, bassValue);
        AdjustEmissionAndLighting(midObject, midValue);
        AdjustEmissionAndLighting(trebleObject, trebleValue);
    }

    void AdjustEmissionAndLighting(GameObject frequencyObject, float amplitudeValue)
    {
        // Assuming each frequency object has exactly one Light component as a child
        Light objectLight = frequencyObject.GetComponentInChildren<Light>();
        // Apply bias to light intensity scale when setting light intensity
        if (objectLight != null)
        {
            // Scale the light intensity by the lightIntensityScale
            objectLight.intensity = lightIntensityScaleBias + amplitudeValue * lightIntensityScale;
        }
    }

    void UpdateVolumeData(int index)
    {
        // Guard clause to prevent out-of-range errors
        if (index < 0 || index >= musicData.smoothed_volume.Count)
            return;

        // Get the volume value at the current index
        float volumeValue = musicData.smoothed_volume[index];

        // Map the volume value to the range of the spotlight intensity
        float intensity = Mathf.Lerp(minSpotlightIntensity, maxSpotlightIntensity, volumeValue);
        spotlight.intensity = intensity;
    }

    void TriggerTransientAnimation(int label)
    {
        UnityEngine.Debug.Log(label);
        if (label >= 0 && label < transientObjects.Length)
        {
            GameObject selectedObject = transientObjects[label];
            Material activeMaterial = activeMaterials[label]; // Get the corresponding active material

            // Stop the previous coroutine if it is still running
            if (transientCoroutines[label] != null)
            {
                StopCoroutine(transientCoroutines[label]);
            }
            transientCoroutines[label] = StartCoroutine(PulseAnimation(selectedObject, activeMaterial, inactiveMaterial, transientAnimationDuration, transientFadeDuration));
        }
    }

    IEnumerator PulseAnimation(GameObject obj, Material activeMaterial, Material inactiveMaterial, float animationDuration, float fadeDuration)
    {
        // Initial setup
        Renderer rend = obj.GetComponent<Renderer>();
        rend.material = activeMaterial;
        Light objLight = obj.GetComponentInChildren<Light>();

        Vector3 initialScale = obj.transform.localScale;
        Vector3 maxScale = initialScale * transientMaxScale;
        float initialIntensity = objLight != null ? objLight.intensity : 0f; // Check if there's a Light component

        // Determine the scaling factor based on how much larger the object is compared to its initial scale
        float scaleMultiplier = 1 + (initialScale.magnitude - initialScales[Array.IndexOf(transientObjects, obj)].magnitude) * scaleDownFactor;

        // Scale up and fade in animation
        for (float t = 0; t < animationDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / animationDuration;
            // Animate the scale with maximum limit
            obj.transform.localScale = Vector3.Lerp(initialScale, Vector3.Min(maxScale, initialScale * (1.25f / scaleMultiplier)), normalizedTime);
            // Fade in the light intensity, if there is a Light component
            if (objLight != null)
            {
                objLight.intensity = Mathf.Lerp(0, initialIntensity, normalizedTime); // Assuming we want to fade in from 0 to initialIntensity
            }
            // Fade in material emission could also be added here if required
            yield return null;
        }


        // Fade out animation
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;

            if (objLight != null)
            {
                objLight.intensity = Mathf.Lerp(initialIntensity, 0, normalizedTime);
            }

            // Calculate scale down speed based on the current scale relative to the initial scale
            float scaleExcess = obj.transform.localScale.magnitude / initialScales[Array.IndexOf(transientObjects, obj)].magnitude;
            float scaleDownSpeed = Mathf.Lerp(1f, scaleDownFactor, scaleExcess - 1f); // Increase scale down factor based on how much larger the scale is than the initial

            // Apply scale down with the variable speed
            obj.transform.localScale = Vector3.Lerp(obj.transform.localScale, initialScales[Array.IndexOf(transientObjects, obj)], normalizedTime * scaleDownSpeed);

            Color currentColor = Color.Lerp(activeMaterial.color, Color.black, normalizedTime);
            rend.material.SetColor("_EmissionColor", currentColor);

            yield return null;
        }

        // Reset to initial state
        rend.material = inactiveMaterial;
        if (objLight != null)
        {
            objLight.intensity = 0f;
            objLight.enabled = false;
        }
        obj.transform.localScale = initialScales[Array.IndexOf(transientObjects, obj)];
    }
}

