using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bop : MonoBehaviour
{

    public GameObject head;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      head.transform.Rotate(Vector3.right, 2);
      head.transform.Rotate(Vector3.right, -2);


    }
}
