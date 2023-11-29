using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triggers : MonoBehaviour
{

    [SerializeField] private Animator animator;
    [SerializeField] private string trigger;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "crank")
        {

            animator.SetBool(trigger, true);
            Destroy(this.gameObject);
        }




    }
}
