using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MaestroArm),typeof(GestureRecognition))]
public class GestureSequenceRecognition : MonoBehaviour {

	[HideInInspector]
	public bool recording;

	public Transform rigHead;

	[HideInInspector]
	public MaestroArm maestroArm;

	[HideInInspector]
	public GestureRecognition gestureRecognition;

	public string sequenceName;
	public string customSubdirectory;

	[HideInInspector]
	public GestureSequence currentSequence;

	public List<GestureSequence> sequences;

	[HideInInspector]
	public GameObject localizer;

	void Awake() {
		if (!rigHead) {
			DebugLogger.DebugError(this.gameObject,"Please populate the RigHead transform field with the rig's head before proceeding.");
		}

		maestroArm = GetComponent<MaestroArm>();
		gestureRecognition = GetComponent<GestureRecognition>();
	}

	public void BeginNewSequence() {
		recording = true;

		localizer = new GameObject();
		localizer.name = "GestureSequenceRecognitionLocalizer";
		localizer.transform.SetParent(rigHead);
	}

	public void StopRecordingSequence() {
		currentSequence = null;

		DestroyImmediate(localizer.gameObject);
		localizer = null;

		recording = false;
	}

	void Update() {
		
	}
}