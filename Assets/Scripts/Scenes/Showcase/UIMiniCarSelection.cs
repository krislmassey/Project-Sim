using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMiniCarSelection : MonoBehaviour {
    private GameObject MiniCar;
    private controllerGrabObjects grabScript;
    private GameObject objectInHand;
    private bool UIActive = false;

    private void Awake()
    {
        grabScript = GetComponent<controllerGrabObjects>();
    }

    void Update () {
        objectInHand = grabScript.GetObjectInHand();

        if (UIActive)
        {
            if(objectInHand == null || objectInHand.tag != "MiniCar")
            {
                //disable UI
            }
            //else leave UI in place
        }
        else
        {
            if (objectInHand != null && objectInHand.tag == "MiniCar")
            {
                //display banner for the mini car with info and/or options
            }
        }
        

        //if minicar is not in hand, display nothing
	}
}
