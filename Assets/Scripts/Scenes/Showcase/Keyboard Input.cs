using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CAVS.Scenes.Showcase {
	public class NewBehaviourScript : MonoBehaviour {
		// Update is called once per frame
		public static void Update() {
			if (Input.GetKeyDown (KeyCode.RightArrow)) {
				//call from that script
				//GameObject.GetComponent<SceneManagerBehavior>().OnButtonPress("Next");
			}
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				//call from that script
				//GameObject.GetComponent<SceneManagerBehavior>().OnButtonPress("Previous");
			}
		}
	}
}
