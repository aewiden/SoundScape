using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedCollide : MonoBehaviour
{
    [SerializeField] Animator sprout;
    [SerializeField] GameObject seed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Seed")
        {
            sprout.SetBool("SeedIn", true);
            seed.SetActive(!seed.activeSelf);
        }
    }
}
