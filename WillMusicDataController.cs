using System.Collections;
using UnityEngine;

public class WillMusicDataController : MonoBehaviour
{
    public AudioSource audioSource;
    public TextAsset jsonFile;
    private MusicData musicData;

    [Header("Beat Objects")]
    public GameObject[] beatObjects;
    public Material defaultMaterial;
    public Material emissiveMaterial;

    [Tooltip("The time it takes for downbeats to fade back to the default material.")]
    public float fadeDuration = 0.3f;
    public float beatAnimationScale = 2;

    [Header("Energy Particle Systems")]
    public ParticleSystem[] highEnergyParticleSystems; // Particle systems active during high energy
    public ParticleSystem[] lowEnergyParticleSystems;  // Particle systems active during low energy

    [Header("Frequency Data Objects")]
    public GameObject bassObject;
    public GameObject midObject;
    public GameObject trebleObject;

    private Vector3 bassOriginalScale;
    private Vector3 midOriginalScale;
    private Vector3 trebleOriginalScale;

    public float maxScale = 2.0f;
    public float maxRotationSpeed = 5.0f;

    [Header("Volume Data Objects")]
    //public Light spotlight;
    public ParticleSystem volumeParticleSystem; // Reference to the particle system
    public float minEmissionRate = 50f; // Minimum emission rate
    public float maxEmissionRate = 500f; // Maximum emission rate

    //[Header("Transient Objects")]
    //public GameObject[] transientObjects; // Should be assigned to 5 objects in the Unity Inspector

    //[Header("Transient Materials")]
    //public Material inactiveMaterial; // Assign the 'inactive' material here
    //public Material[] activeMaterials; // Assign 5 'active' materials here, one for each object

    //[Header("Transient Animation Control")]
    //public float transientMaxScale = 3f; // Maximum allowed scale
    //public float scaleDownFactor = 0.1f; // Factor to scale down the objects that are too large
    //public float transientAnimationDuration = 1.0f; // Duration of the transient pulse animation
    //public float transientFadeDuration = 0.5f; // Duration of the fade out for transient animation

    // Beat private variables
    private Light[] beatLights;
    private float[] initialLightIntensities; // Array to store initial intensities of each light
    private Coroutine[] fadeCoroutines; // Array to keep track of the fade coroutines for each beat object
    private Vector3[] beatInitialScales;
    private Color initialEmissionColor;

    // This will help us not to check each beat again and again
    private int nextBeatIndex = 0;

    // Use these indexes to keep track of high and low energy beats
    private int nextHighEnergyIndex = 0;
    private int nextLowEnergyIndex = 0;

    private float dataInterval; // Time interval between each data point

    // Frequency private variables 
    private float bassScaleSpeed;
    private float midScaleSpeed;
    private float trebleScaleSpeed;

    private float bassRotationSpeed;
    private float midRotationSpeed;
    private float trebleRotationSpeed;

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
        StartCoroutine(StartAfterDelay(2.0f)); // Delay for 2 seconds before starting everything
        //PlayMusic();

        // Initialize beatLights array
        beatLights = new Light[beatObjects.Length];
        initialLightIntensities = new float[beatObjects.Length];
        fadeCoroutines = new Coroutine[beatObjects.Length];
        beatInitialScales = new Vector3[beatObjects.Length]; // Initialize the initial scales array

        for (int i = 0; i < beatObjects.Length; i++)
        {
            beatLights[i] = beatObjects[i].transform.GetChild(0).GetComponent<Light>();
            initialLightIntensities[i] = beatLights[i].intensity;
            beatInitialScales[i] = beatObjects[i].transform.localScale; // Store the initial scale of each object
        }

        initialEmissionColor = beatObjects[0].GetComponent<Renderer>().material.GetColor("_EmissionColor");

        bassOriginalScale = bassObject.transform.localScale;
        midOriginalScale = midObject.transform.localScale;
        trebleOriginalScale = trebleObject.transform.localScale;

        // Assume `songDuration` is the total duration of the song in seconds
        float songDuration = audioSource.clip.length;
        // Calculate the time interval between each data point
        dataInterval = songDuration / musicData.frequency_data.bass.amplitude_values.Count;

        // Initialize the volume data interval
        volumeDataInterval = songDuration / musicData.smoothed_volume.Count;

        //transientCoroutines = new Coroutine[transientObjects.Length]; // Initialize the array based on the number of transient objects

        //// Initialize the initialScales array to store the original scale of each transient object
        //initialScales = new Vector3[transientObjects.Length];
        //for (int i = 0; i < transientObjects.Length; i++)
        //{
        //    initialScales[i] = transientObjects[i].transform.localScale;
        //}
    }

    IEnumerator StartAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayMusic();
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

        //// Check transients using the index and threshold
        //while (nextTransientIndex < musicData.transients.onset_times.Count)
        //{
        //    float nextTransientTime = musicData.transients.onset_times[nextTransientIndex];

        //    if (currentTime >= nextTransientTime - TIME_THRESHOLD && currentTime <= nextTransientTime + TIME_THRESHOLD)
        //    {
        //        // Trigger transient animation if we're within the threshold window
        //        int label = musicData.transients.labels[nextTransientIndex];
        //        TriggerTransientAnimation(label);
        //        nextTransientIndex++; // Move to the next transient
        //    }
        //    else if (currentTime > nextTransientTime + TIME_THRESHOLD)
        //    {
        //        // Skip this transient if we've passed the threshold window
        //        nextTransientIndex++;
        //    }
        //    else
        //    {
        //        // If we're not yet at the next transient time, exit the loop
        //        break;
        //    }
        //}
    }

    void TriggerBeatAnimation()
    {
        for (int i = 0; i < beatObjects.Length; i++)
        {
            beatObjects[i].GetComponent<Renderer>().material = emissiveMaterial;
            beatLights[i].enabled = true;

            if (fadeCoroutines[i] != null)
            {
                StopCoroutine(fadeCoroutines[i]);
            }

            fadeCoroutines[i] = StartCoroutine(FadeOutLightAndMaterial(beatObjects[i], beatLights[i], i));
        }
    }

    IEnumerator FadeOutLightAndMaterial(GameObject beatObject, Light beatLight, int index)
    {
        Vector3 initialScale = beatInitialScales[index]; // Use the stored initial scale
        Vector3 targetScale = initialScale * beatAnimationScale; // Calculate the target scale

        Material objectMaterial = beatObject.GetComponent<Renderer>().material;
        Color targetEmissionColor = initialEmissionColor * Mathf.LinearToGammaSpace(0); // Faded emission color

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            beatLight.intensity = Mathf.Lerp(initialLightIntensities[index], 0, normalizedTime);
            beatObject.transform.localScale = Vector3.Lerp(targetScale, initialScale, normalizedTime);

            // Interpolate the emission color's brightness
            Color interpolatedEmissionColor = Color.Lerp(targetEmissionColor, initialEmissionColor, normalizedTime);
            objectMaterial.SetColor("_EmissionColor", interpolatedEmissionColor);

            yield return null;
        }

        beatLight.enabled = false;
        objectMaterial.SetColor("_EmissionColor", initialEmissionColor); // Reset emission color
        ResetMaterial(beatObject);
        beatLight.intensity = initialLightIntensities[index];
        beatObject.transform.localScale = initialScale;
    }

    void ResetMaterial(GameObject beatObject)
    {
        beatObject.GetComponent<Renderer>().material = defaultMaterial;
    }

    void TriggerHighEnergyState()
    {
        // Activate high energy particle systems
        foreach (ParticleSystem ps in highEnergyParticleSystems)
        {
            ps.Play();
        }

        // Deactivate low energy particle systems
        foreach (ParticleSystem ps in lowEnergyParticleSystems)
        {
            ps.Stop();
        }
    }

    void TriggerLowEnergyState()
    {
        // Deactivate high energy particle systems
        foreach (ParticleSystem ps in highEnergyParticleSystems)
        {
            ps.Stop();
        }

        // Activate low energy particle systems
        foreach (ParticleSystem ps in lowEnergyParticleSystems)
        {
            ps.Play();
        }
    }

    void UpdateFrequencyData(int index)
    {
        if (index < 0 || index >= musicData.frequency_data.bass.amplitude_values.Count)
            return;

        bassScaleSpeed = musicData.frequency_data.bass.amplitude_values[index] * maxScale;
        midScaleSpeed = musicData.frequency_data.mid.amplitude_values[index] * maxScale;
        trebleScaleSpeed = musicData.frequency_data.treble.amplitude_values[index] * maxScale;

        bassRotationSpeed = musicData.frequency_data.bass.amplitude_values[index] * maxRotationSpeed;
        midRotationSpeed = musicData.frequency_data.mid.amplitude_values[index] * maxRotationSpeed;
        trebleRotationSpeed = musicData.frequency_data.treble.amplitude_values[index] * maxRotationSpeed;
    }

    void Update()
    {
        RotateAndScaleObject(bassObject, bassRotationSpeed, bassScaleSpeed, bassOriginalScale);
        RotateAndScaleObject(midObject, midRotationSpeed, midScaleSpeed, midOriginalScale);
        RotateAndScaleObject(trebleObject, trebleRotationSpeed, trebleScaleSpeed, trebleOriginalScale);
    }

    private void RotateAndScaleObject(GameObject obj, float rotationSpeed, float scaleSpeed, Vector3 originalScale)
    {
        obj.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);

        float scaleChange = Mathf.Sin(Time.time * scaleSpeed) * 0.5f + 0.5f;
        Vector3 targetScale = originalScale * (1 + scaleChange * (maxScale - 1));
        obj.transform.localScale = targetScale;
    }

    void UpdateVolumeData(int index)
    {
        if (index < 0 || index >= musicData.smoothed_volume.Count)
            return;

        float volumeValue = musicData.smoothed_volume[index];

        //// Update spotlight intensity (existing functionality)
        //float intensity = Mathf.Lerp(minEmissionRate, maxEmissionRate, volumeValue);
        //spotlight.intensity = intensity;

        // Update particle system emission rate based on volume
        var emissionModule = volumeParticleSystem.emission;
        float emissionRate = Mathf.Lerp(minEmissionRate, maxEmissionRate, volumeValue);
        emissionModule.rateOverTime = emissionRate;
    }


    //void TriggerTransientAnimation(int label)
    //{
    //    UnityEngine.Debug.Log(label);
    //    if (label >= 0 && label < transientObjects.Length)
    //    {
    //        GameObject selectedObject = transientObjects[label];
    //        Material activeMaterial = activeMaterials[label]; // Get the corresponding active material

    //        // Stop the previous coroutine if it is still running
    //        if (transientCoroutines[label] != null)
    //        {
    //            StopCoroutine(transientCoroutines[label]);
    //        }
    //        transientCoroutines[label] = StartCoroutine(PulseAnimation(selectedObject, activeMaterial, inactiveMaterial, transientAnimationDuration, transientFadeDuration));
    //    }
    //}

    //IEnumerator PulseAnimation(GameObject obj, Material activeMaterial, Material inactiveMaterial, float animationDuration, float fadeDuration)
    //{
    //    // Initial setup
    //    Renderer rend = obj.GetComponent<Renderer>();
    //    rend.material = activeMaterial;
    //    Light objLight = obj.GetComponentInChildren<Light>();

    //    Vector3 initialScale = obj.transform.localScale;
    //    Vector3 maxScale = initialScale * transientMaxScale;
    //    float initialIntensity = objLight != null ? objLight.intensity : 0f; // Check if there's a Light component

    //    // Determine the scaling factor based on how much larger the object is compared to its initial scale
    //    float scaleMultiplier = 1 + (initialScale.magnitude - initialScales[Array.IndexOf(transientObjects, obj)].magnitude) * scaleDownFactor;

    //    // Scale up and fade in animation
    //    for (float t = 0; t < animationDuration; t += Time.deltaTime)
    //    {
    //        float normalizedTime = t / animationDuration;
    //        // Animate the scale with maximum limit
    //        obj.transform.localScale = Vector3.Lerp(initialScale, Vector3.Min(maxScale, initialScale * (1.25f / scaleMultiplier)), normalizedTime);
    //        // Fade in the light intensity, if there is a Light component
    //        if (objLight != null)
    //        {
    //            objLight.intensity = Mathf.Lerp(0, initialIntensity, normalizedTime); // Assuming we want to fade in from 0 to initialIntensity
    //        }
    //        // Fade in material emission could also be added here if required
    //        yield return null;
    //    }

    //    // Fade out animation
    //    for (float t = 0; t < fadeDuration; t += Time.deltaTime)
    //    {
    //        float normalizedTime = t / fadeDuration;

    //        if (objLight != null)
    //        {
    //            objLight.intensity = Mathf.Lerp(initialIntensity, 0, normalizedTime);
    //        }

    //        // Calculate scale down speed based on the current scale relative to the initial scale
    //        float scaleExcess = obj.transform.localScale.magnitude / initialScales[Array.IndexOf(transientObjects, obj)].magnitude;
    //        float scaleDownSpeed = Mathf.Lerp(1f, scaleDownFactor, scaleExcess - 1f); // Increase scale down factor based on how much larger the scale is than the initial

    //        // Apply scale down with the variable speed
    //        obj.transform.localScale = Vector3.Lerp(obj.transform.localScale, initialScales[Array.IndexOf(transientObjects, obj)], normalizedTime * scaleDownSpeed);

    //        Color currentColor = Color.Lerp(activeMaterial.color, Color.black, normalizedTime);
    //        rend.material.SetColor("_EmissionColor", currentColor);

    //        yield return null;
    //    }

    //    // Reset to initial state
    //    rend.material = inactiveMaterial;
    //    if (objLight != null)
    //    {
    //        objLight.intensity = 0f;
    //        objLight.enabled = false;
    //    }
    //    obj.transform.localScale = initialScales[Array.IndexOf(transientObjects, obj)];
    //}
}
