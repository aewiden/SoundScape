// // ************************************************************************************************************
// // CompManager      - compositionManager for an example of using singletons with scriptable objects for DXR F22
// //                  - code for generating a structure based on input data from scriptable object "kit"
// // by James Mahoney - September 7, 2022
// //                    bug fixes - September 8, 2022
// // ************************************************************************************************************
//
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class CompManager : MonoBehaviour
// {
//     public GameObject spawnPoint;       // extra credit parent new objects to this spawn point
//     public Kit curKit;                  // kit of parts from scriptable object data
//     public Material materialGen;        // custom shader
//     public float minSmooth, maxSmooth;  // how smooth should the new element be?
//     public float minScale, maxScale;    //  in some cases, allow the scale to be randomized between these values
//     public float chanceRotation;        // sometimes rotate the new element
//     public float maxJitterXZ;           // jitter the position around some
//     public int stories;                 // how tall is this thing anyway?
//     public float jitterHeight;          // jitter the y position some too
//     private Renderer rend;              // need a handle to the new elements renderer component
//     private List<GameObject> elementList; // keep a list of all the generated elements
//
//     // Start is called before the first frame update
//     void Start()
//     {
//        // initialize anything that might need it here
//     }
//
//     // add a new element to the composition
//     public void Add_Element()
//     {
//         if (curKit.kitparts.Length == 0)
//         {
//             Debug.Log("kit is empty");
//             return; // empty kit - nothing to do
//         }
//
//         // check to see if this is the first element
//         if (elementList == null)
//         {
//             // set up new list to hold elements
//             elementList = new List<GameObject>();
//         }
//
//         // choose a random part from input array of possible parts in the kit
//         int curPart = Random.Range(0, curKit.kitparts.Length);
//
//         // jitter rotation
//         Quaternion qrot; //rotation angles
//         if (Random.Range(0.0f, 1.0f) < chanceRotation && curKit.kitparts[curPart].rotateYoptions.angles.Length > 0)
//         {
//             // choose a y rotation option from the list for the chosen part
//             int rotIndex = Random.Range(0, curKit.kitparts[curPart].rotateYoptions.angles.Length);
//             float rotY = curKit.kitparts[curPart].rotateYoptions.angles[rotIndex];
//             qrot = Quaternion.Euler(0, rotY, 0); //only rotate around y axis
//
//         }
//         else
//             qrot = Quaternion.identity; // no rotation
//
//         // jitter position
//         Vector3 newPos;
//         int randSign = Random.value < .5 ? 1 : -1;
//         float randVal = Random.value * maxJitterXZ;
//         newPos.x = randVal * randSign;          // jitter x position
//         randSign = Random.value < .5 ? 1 : -1;
//         randVal = Random.value * maxJitterXZ;
//         randSign = Random.value < .5 ? 1 : -1;  // new random sign
//         newPos.z = randVal * randSign;          // jitter z position
//         newPos.y = 0.0f;
//         if (!curKit.kitparts[curPart].isBase)
//         {
//             // if not base move element up along y based on new random floor and a little jitter
//             // floor height is hard set to 4 units
//             randSign = Random.value < .5 ? 1 : -1;  // new random sign
//             newPos.y = (float)Random.Range(0, stories) * 4.0f + randSign * Random.Range(0.0f, jitterHeight);
//         }
//
//         // instantiate new object
//         GameObject newObj;
//         newObj = GameObject.Instantiate(curKit.kitparts[curPart].form, newPos, qrot) as GameObject;
//
//         // add element to list
//         elementList.Add(newObj);
//
//         // jitter scale
//         if (curKit.kitparts[curPart].canScale)
//         {
//             float newScale = Random.Range(minScale, maxScale);
//             newObj.transform.localScale = new Vector3(newScale, newScale, newScale);
//         }
//
//         // jitter color to random value from palette image
//
//         // set material
//         rend = newObj.GetComponent<Renderer>(); // renderer component is created when element is instantated
//         rend.material = materialGen;            // assign to custom shader material (shader graph)
//
//         //set palette
//         rend.material.SetTexture("_palette", curKit.palette); //_palette is an input value for this shader graph
//
//         // choose color from palette by indexing to a random pixel of the input image - "the palette"
//         Vector2 colorIndex;
//         colorIndex.x = Random.Range(0.0f, 1.0f);
//         colorIndex.y = Random.Range(0.0f, 1.0f);
//         rend.material.SetVector("_colorIndex", colorIndex); //_colorIndex is an input value for this shader graph
//         if (curKit.kitparts[curPart].textureOptions.texture.Length > 0)
//         {
//             // set texture input value used by this custom shader graph material
//             int tex = Random.Range(0, curKit.kitparts[curPart].textureOptions.texture.Length);
//             rend.material.SetTexture("_texture", curKit.kitparts[curPart].textureOptions.texture[tex]);
//         }
//
//         // jitter smoothness
//         float smoothness = Random.Range(minSmooth, maxSmooth);
//         rend.material.SetFloat("_smoothness", smoothness);
//     }
//
//     public void DeleteLastElement()
//     {
//         if (elementList.Count > 0)
//         {
//             //elementList[elementList.Count - 1].SetActive(false); // hide object from scene
//             Destroy(elementList[elementList.Count - 1]);        // delete object from scene
//             elementList.RemoveAt(elementList.Count - 1);        // remove object from list
//         }
//     }
//     public void WriteData() // extra credit - totally not necessary
//     {
//
//     }
//
// }
