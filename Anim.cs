using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim : MonoBehaviour
{

    public GameObject hand;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      //might be the shape element ...
      // if(hand.Gesture == "Point")
      // {
      //   //condition here
      // }
      animator.SetBool("Play", true);

    }
}
