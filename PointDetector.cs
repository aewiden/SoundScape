using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class PointDetector : MonoBehaviour
{
    [SerializeField] Animator animation;
    [SerializeField] string trigger;
    [SerializeField] AudioSource wind;
    
    public void SetAnimBool()
    {
        animation.SetBool(trigger, true);
        wind.Play();
    }
    public void SetAnimBoolFalse()
    {
        animation.SetBool(trigger, false);
    }
}
