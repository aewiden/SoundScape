using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float speed = 60;
        Vector3 rot = new Vector3(0, 90, 0);
        this.transform.Rotate(rot, speed * Time.deltaTime);  
    }
}
