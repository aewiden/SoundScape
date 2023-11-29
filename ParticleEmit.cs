using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEmit : MonoBehaviour
{
    public Transform leftHand;
    public Transform rightHand;
    public ParticleSystem particleSystem;
    public float emissionRateMultiplier = 10.0f;
    public float minEmissionRate = 1.0f;
    public float maxEmissionRate = 20.0f;
    public float startSpeedMultiplier = 5.0f;
    public float minStartSpeed = 1.0f;
    public float maxStartSpeed = 10.0f;

    // Update is called once per frame
    void Update()
    {
        if (leftHand != null && rightHand != null)
        {
            // Get the direction from the left hand to the right hand
            Vector3 handDirection = rightHand.position - leftHand.position;

            // Calculate the emission rate based on the distance between hands
            float distance = handDirection.magnitude;
            float emissionRate = MapDistanceToEmissionRate(distance);

            // Calculate the start speed based on the distance between hands
            float startSpeed = MapDistanceToStartSpeed(distance);

            // Apply the adjusted values to the Particle System
            var mainModule = particleSystem.main;
            mainModule.maxParticles = Mathf.CeilToInt(emissionRate * emissionRateMultiplier);
            mainModule.startSpeed = startSpeed;
        }
    }

    float MapDistanceToEmissionRate(float distance)
    {
        // Map the distance between hands to emission rate
        float mappedEmissionRate = Mathf.Lerp(minEmissionRate, maxEmissionRate, distance / maxEmissionRate);

        return Mathf.Clamp(mappedEmissionRate, minEmissionRate, maxEmissionRate);
    }

    float MapDistanceToStartSpeed(float distance)
    {
        // Map the distance between hands to start speed
        float mappedStartSpeed = Mathf.Lerp(minStartSpeed, maxStartSpeed, distance / maxEmissionRate);

        return Mathf.Clamp(mappedStartSpeed, minStartSpeed, maxStartSpeed);
    }
}
