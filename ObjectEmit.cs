using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEmit : MonoBehaviour
{

    public Transform leftHand;
    public Transform rightHand;
    public Material material;
    public float emissionMultiplier = 1.0f;
    public float minEmission = 1.0f;
    public float maxEmission = 10.0f;

    // Update is called once per frame
    void Update()
    {
        if (leftHand != null && rightHand != null)
        {
            // Get the direction from the left hand to the right hand
            Vector3 handDirection = rightHand.position - leftHand.position;

            // Calculate the distance between hands
            float distance = handDirection.magnitude;

            // Map the distance to emission
            float emission = MapDistanceToEmission(distance);

            // Apply the adjusted emission to the material
            material.SetColor("_EmissionColor", new Color(emission, emission, emission));
        }
    }

    float MapDistanceToEmission(float distance)
    {
        // Map the distance between hands to emission
        float mappedEmission = Mathf.Lerp(minEmission, maxEmission, distance / maxEmission);

        return Mathf.Clamp(mappedEmission, minEmission, maxEmission) * emissionMultiplier;
    }
}