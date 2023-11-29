using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSynth : MonoBehaviour
{

    public Transform leftHand;
    public Transform rightHand;
    public float rotationSpeed = 5.0f;

    // Update is called once per frame
    void Update()
    {
        if (leftHand != null && rightHand != null)
        {
            // Get the direction from the left hand to the right hand
            Vector3 handDirection = rightHand.position - leftHand.position;

            // Calculate the rotation angle based on the direction
            float rotationAngle = Mathf.Atan2(handDirection.y, handDirection.x) * Mathf.Rad2Deg;

            // Apply the rotation to the object
            transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle * rotationSpeed);
        }
    }
}
