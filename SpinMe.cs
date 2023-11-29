using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinMe : MonoBehaviour
{
    public GameObject gm;
    public float Speed= 1.0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


        gm.transform.Rotate(Vector3.up*Speed*Time.deltaTime);
    }
}
