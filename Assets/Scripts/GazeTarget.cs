using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class GazeTarget : MonoBehaviour {

	public float timerLength; // How long this gaze must be held to complete
	public Color circleColor; // The color the GazeCircle will be made. Generally Green for affirmative or Red for negative or White for neutral.

	public bool circleVisible;
	public bool destroyOnComplete;

	public UnityEvent OnTargetBegin;
	public UnityEvent OnTargetComplete;
	public UnityEvent OnTargetCancel;

	public void CanBeGazed() {
		gameObject.layer = 8;
	}

	public void CantBeGazed() {
		gameObject.layer = 2;
	}

	public void GazeComplete() {
		OnTargetComplete.Invoke();

		if (destroyOnComplete) {
			CantBeGazed ();
			Destroy (this);
		}
	}
}
