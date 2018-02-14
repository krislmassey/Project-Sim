using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CAVS.Scenes.Showcase {
	public class KeyboardInput: MonoBehaviour {
		
        public void Update() {
			if (Input.GetKeyDown (KeyCode.RightArrow)) {
				//call from that script
				gameObject.GetComponent<SceneManagerBehavior>().OnButtonPress("Next");
			}
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				//call from that script
				gameObject.GetComponent<SceneManagerBehavior>().OnButtonPress("Previous");
			}
		}

	}
}
