using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuActivation : MonoBehaviour {

	public string keyGesture;

	public void GestureListener(GestureObject gesture) {
		if (string.Equals(gesture.gestureName, keyGesture)) {
			GetComponent<MaestroManager>().ActivateMainSettings();
		}
	}
}
