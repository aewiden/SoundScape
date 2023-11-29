using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RotatingObjectWithBlur : MonoBehaviour
{
    public GameObject parent;
    public float blurRotation = 0f;


    void Start()
    {
        StartCoroutine(RotateObj());  
    }



    IEnumerator RotateObj()
    {
        for(int i =0; i < 10; i++)
        {
            //get the point,
            GameObject curr = parent.transform.GetChild(i).gameObject;
           

            //rotate it about the x using parent position
            //add rotation angle
            Vector3 pivot = parent.transform.position;

            curr.transform.RotateAround(pivot, Vector3.right, blurRotation);

            blurRotation += 20;
            yield return new WaitForSeconds(0.01f);

        }
        //derotate
        for(int i = 9; i >= 0; i--)
        {
            GameObject curr = parent.transform.GetChild(i).gameObject;


            //rotate it about the x using parent position
            //add rotation angle
            Vector3 pivot = parent.transform.position;
            blurRotation -= 20;
            curr.transform.RotateAround(pivot, -Vector3.right, blurRotation);

            
            yield return new WaitForSeconds(0.01f);
        }
    }


}
