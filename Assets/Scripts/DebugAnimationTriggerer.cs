using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugAnimationTriggerer : MonoBehaviour {

	public string triggerName;
	
	void Update () {
		if (Input.GetMouseButtonDown(2)) {
			GetComponent<Animator>().SetTrigger(triggerName);
		}
	}
}
