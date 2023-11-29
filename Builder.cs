using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    //displace
    //rotate
    //and scale. position of the pivot for rotation)
    public float scaleFactor = 3f;
    public float dispFactor = 3f;
    public float rotateFactor = 360f;
    public float blurRotation = 0f;
    public float rotationSpeed = 1.0f;

    public GameObject parent;
    public float spiralDisp = 0.05f;
    private Vector3 ranAxis;

    private int numIterations = 1;
    private float heightOffset = 1.5f;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(runInstances());



    }

    private void FixedUpdate()
    {
       
        parent.transform.Rotate(Vector3.up * rotationSpeed);
    }

    IEnumerator runInstances()
    {
     
        //StartCoroutine(genSpiral());
        for (int i = 0; i < numIterations; i++)
        {
            yield return genSpiral();
            yield return new WaitForSeconds(4f);
            // yield return genSpiral();
            //parent.transform.Rotate(Vector3.up *rotateFactor);
            //yield return build();
            //StartCoroutine(build());
        }
    }

    IEnumerator build()
    {

        for (int i = 0; i <= 6; i++)
        {
            GameObject curr = parent.transform.GetChild(i).gameObject;
            Debug.Log(curr);
            RanDisplace(curr);
            RanScale(curr);
            RanRotate(curr);

            yield return new WaitForSeconds(.1f);


            //scale
            //displace
        }

        StartCoroutine(RotateObj());
        yield return new WaitForSeconds(.1f);

    }



    IEnumerator RotateObj()
    {
        for (int i = 0; i < 10; i++)
        {
            //get the point,
            GameObject curr = parent.transform.GetChild(i).gameObject;


            //rotate it about the x using parent position
            //add rotation angle
            Vector3 pivot = parent.transform.position;

            curr.transform.RotateAround(pivot, Vector3.right, blurRotation);

            blurRotation += 20;
            yield return new WaitForSeconds(0.1f);

        }
        //derotate
        for (int i = 9; i >= 0; i--)
        {
            GameObject curr = parent.transform.GetChild(i).gameObject;


            //rotate it about the x using parent position
            //add rotation angle
            Vector3 pivot = parent.transform.position;
            blurRotation -= 20;
            curr.transform.RotateAround(pivot, -Vector3.right, blurRotation);


            yield return new WaitForSeconds(0.1f);
        }
    }


    int Mod(int a, int n) => (a % n + n) % n;

    void RanScale(GameObject curr)
    {
        //Debug.Log("In scale");
        randomAxis();

        //we can set a range of random scaling

        curr.transform.localScale+= ranAxis * (scaleFactor+randomizer());
       // yield return new WaitForSeconds(0.01f);
    }


    IEnumerator genSpiral()
    {

        int numObjects = parent.transform.childCount;
      
        for (int i =0; i < numObjects; i++)
        {
            float angle = i * 360.0f / numObjects; // Calculate the angle for placement
            

            GameObject curr = parent.transform.GetChild(i).gameObject;
            Spiral(curr,  angle, i);
          
            yield return new WaitForSeconds(.1f);
        }
        //StartCoroutine(reverseSpiral());

        //StartCoroutine(Spring());
       


    }
    IEnumerator reverseSpiral()
    {
        int numObjects = parent.transform.childCount;
        int count = 0;
        for (int i = numObjects-1; i >=0; i--)
        {

            count++;
            if (count == 1)
            {
                heightOffset = -heightOffset;
            }

            float angle = i * 360.0f / numObjects; // Calculate the angle for placement


            GameObject curr = parent.transform.GetChild(i).gameObject;
            if (count == 1)
            {
                DestroyObject(curr);
            }
           
            Spiral(curr, angle, i);
            yield return new WaitForSeconds(.1f);
            
        }

    }

    void Spiral(GameObject curr,float angle, int idx)
    {

        float radians = angle * Mathf.Deg2Rad; // Convert angle to radians
        // Calculate the position in a spiral pattern
        float x = Mathf.Cos(radians)  * idx;
        float y = idx * heightOffset;
        float z = Mathf.Sin(radians) *  idx;

        // Create the object and set its position

        curr.transform.position += new Vector3(x, y, z);
        // Calculate rotation to look towards the center
        //pivot at the abs centre, can change to sth else
        Quaternion rot = Quaternion.LookRotation(Vector3.zero - curr.transform.position);
        curr.transform.rotation = rot;

    }

     IEnumerator Spring()
    {
        //bounce and lag
        //we can scale and just flip the direction
        //calculate the centre point and have it go up and down, lag when you get up, and then get down fast

        //num of kids, set the pivot point as the centre 
        int numObjects = parent.transform.childCount;
        int centre = numObjects / 2;
        float waitTime = 0.1f;

      Vector3 pivot = parent.transform.GetChild(centre).gameObject.transform.position;
     

        for(int i = numObjects-1; i >centre; i--)
        {
            GameObject down = parent.transform.GetChild(i).gameObject;
            GameObject up = parent.transform.GetChild(numObjects - i - 1).gameObject;

            
            down.transform.position += Vector3.up * 2;
            up.transform.position += Vector3.down * 2;
            yield return new WaitForSeconds(waitTime);
            waitTime /= 2f;
        }
        //after the up
        yield return new WaitForSeconds(0.2f);
        for (int i = numObjects - 1; i > centre; i--)
        {
            GameObject down = parent.transform.GetChild(i).gameObject;
            GameObject up = parent.transform.GetChild(numObjects - i - 1).gameObject;


            down.transform.position += Vector3.down * 2;
            up.transform.position += Vector3.up * 2;
            yield return new WaitForSeconds(waitTime);
            waitTime /= 2f;
        }


        //delay the coming back down




    }

    void RanRotate(GameObject curr)
    {
        //Debug.Log("In rotate");
        randomAxis();

        //we can set a range of random scaling

        curr.transform.Rotate(ranAxis * (rotateFactor + randomizer()));
        // yield return new WaitForSeconds(0.01f);
    }

    void RanDisplace(GameObject curr)
    {
      //  Debug.Log("In displace");

        randomAxis();

        //we can set a range of random scaling
      
        curr.transform.position += ranAxis * (dispFactor+randomizer() );
        //yield return new WaitForSeconds(4);
    }

    float randomizer()
    {
        int number = Random.Range(2, 19);

       // int number = 10; // Replace 10 with your desired number
        bool isPositive = Random.Range(0, 2) == 0; // Generates a random boolean value

        if (!isPositive)
        {
            number = -number; // Reverses the sign if the boolean is false
        }
        return number;
    }

    void  randomAxis()
    {
        //we can randomise scaling in x, y or z
        int randomNumber = Random.Range(0, 6); // Generates a random number between 0 (inclusive) and 3 (exclusive)
        Debug.Log(randomNumber);
        randomNumber = Mod(randomNumber + 1, 6);

        if (randomNumber == 0) //x
        {
            ranAxis = Vector3.right;
        }
        if (randomNumber == 1)//y
        {
            ranAxis = Vector3.up;
        }
        if (randomNumber == 2) //x
        {
            ranAxis = Vector3.forward;
        }
        if (randomNumber == 3) //x
        {
            ranAxis = Vector3.down;
        }
        if (randomNumber == 4) //x
        {
            ranAxis = Vector3.left;
        }
        else
        {
            ranAxis = Vector3.back;

        }


    }
}
