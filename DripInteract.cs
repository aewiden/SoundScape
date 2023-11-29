using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DripInteract : MonoBehaviour
{
    public Material dripMat;
    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            if(Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit)){
                dripMat.SetVector("_Coordinates", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0,0));
            }
           
        }
       
        
    }
}
