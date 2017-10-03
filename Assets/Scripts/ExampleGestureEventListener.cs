using UnityEngine;
using System.Collections;

public class ExampleGestureEventListener : MonoBehaviour {

	public Material demoSkybox;

	public bool inTransition;

	/*--------------------------------------------------------------------------------------
	* This is an example script demonstrating how to have a script track events from the MaestroGesture system.
	* 
	* Setting up a script to react to a GestureEvent is very simple.
	*
	* In OnEnable, simply add your function of choice as a listener to any of the predefined GestureEvents or make your own.
	* In OnDisable, make sure to remove your function as well to prevent lost callbacks.
	*
	* In no particular order, these are the default events:
	* 
	* OnGestureBegin - Occurs when a gesture is recognised and sends the new gestureObject with it. Make sure listeners accept a GestureObject parameter!
	* OnGestureEnd - Occurs when the current finger arrangement is unidentifiable. Passes the gesture that just ended.
	--------------------------------------------------------------------------------------*/
	
	public GestureRecognition gestureRecogniser;
		
	void OnEnable() {
		
		gestureRecogniser.OnGestureBegin.AddListener(BeginTest);
		gestureRecogniser.OnGestureEnd.AddListener(EndTest);	

	}
	
	void OnDisable() {
		
		gestureRecogniser.OnGestureBegin.RemoveListener(BeginTest);
		gestureRecogniser.OnGestureEnd.RemoveListener(EndTest);

	}
	
	void BeginTest(GestureObject gesture) {
		Debug.Log("ExampleGestureEventListener: I just noticed you made the " + gesture.gestureName + " gesture, congratulations!");

		if (!inTransition) {
			IEnumerator transition = TransitionSkybox(gesture.gestureName);
			StartCoroutine(transition);
		}
	}
	
	void EndTest(GestureObject gesture) {
		Debug.Log("ExampleGestureEventListener: You just stopped making the " + gesture.gestureName + " gesture.");
	}

	IEnumerator TransitionSkybox(string gestureName) {
		inTransition = true;

		Color fromColor = demoSkybox.GetColor("_SkyTint");
		Color toColor = new Color();

		switch (gestureName) {
			case "Paper":
				toColor = Color.red;
				break;
			case "Scissors":
				toColor = Color.green;
				break;
			case "Rock":
				toColor = Color.white;
				break;
		}

		float i = 0;

		while (i < 1f) {
			i += Time.deltaTime;
			demoSkybox.SetColor("_SkyTint",Color.Lerp(fromColor,toColor,i));
			yield return new WaitForEndOfFrame();
		}

		inTransition = false;

		yield return null;
	} 
}