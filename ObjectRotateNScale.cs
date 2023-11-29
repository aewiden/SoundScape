using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotateNScale : MonoBehaviour
{

    public float rotationSpeed = 5.0f;
    public Vector3 rotationAxis = Vector3.up;

    public float scaleSpeed = 1.0f;
    public float maxScale = 2.0f;

    private Vector3 originalScale;


    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {

        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.Self);

        float scaleChange = Mathf.Sin(Time.time * scaleSpeed) * 0.5f + 0.5f;

        Vector3 targetScale = originalScale + (originalScale * maxScale - originalScale) * scaleChange;
        transform.localScale = targetScale;
        
    }
}
