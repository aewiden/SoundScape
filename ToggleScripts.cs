using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToggleScripts : MonoBehaviour
{
    public MonoBehaviour targetScript;
    private bool isScriptActive = false;
    private OVRHand hand;

    // Start is called before the first frame update
    void Start()
    {
        if (targetScript == null)
        {
            enabled = false;
        }

        hand = GetComponent<OVRHand>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(hand.IsTracked && hand.GetFingerIsPinching(OVRHand.HandFinger.Index))
        {
            ToggleScript();
        }
    }

    void ToggleScript()
    {
        isScriptActive = !isScriptActive;
        targetScript.enabled = isScriptActive;
    }
}
