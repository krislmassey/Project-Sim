using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(GestureSequenceRecognition))]

public class GestureSequenceRecognitionEditor : Editor {

	public GestureSequenceRecognition gestureSequenceRecognition;

	public GestureSequence currentSequence;

	public int recordingChapter;

	void OnEnable() {
		gestureSequenceRecognition = (GestureSequenceRecognition) target;
	}

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		if (!gestureSequenceRecognition.recording) {
			if (GUILayout.Button("Create New Sequence")) {
				recordingChapter = 0;

				currentSequence = ScriptableObject.CreateInstance<GestureSequence>();

				currentSequence.sequenceName = gestureSequenceRecognition.sequenceName;

				currentSequence.considerTime = new List<bool>();
				currentSequence.considerGesture = new List<bool>();
				currentSequence.considerPosition = new List<bool>();
				currentSequence.considerRotation = new List<bool>();

				currentSequence.gestureAtPoint = new List<string>();
				currentSequence.localPositionAtPoint = new List<Vector3>();
				currentSequence.localRotationAtPoint = new List<Quaternion>();

				currentSequence.positionalLeniency = new List<float>();
				currentSequence.rotationalLeniency = new List<float>();

				currentSequence.timeLimit = new List<float>();

				gestureSequenceRecognition.currentSequence = currentSequence;

				gestureSequenceRecognition.BeginNewSequence();
			}
		} else {
			if (GUILayout.Button("Cancel New Sequence")) {
				currentSequence = null;
				gestureSequenceRecognition.StopRecordingSequence();
			}

			if (GUILayout.Button("Create Sequence Point")) {
				if (gestureSequenceRecognition.recording) {
					gestureSequenceRecognition.currentSequence.considerTime.Add(true);
					gestureSequenceRecognition.currentSequence.considerGesture.Add(true);
					gestureSequenceRecognition.currentSequence.considerPosition.Add(true);
					gestureSequenceRecognition.currentSequence.considerRotation.Add(true);

					gestureSequenceRecognition.currentSequence.gestureAtPoint.Add(gestureSequenceRecognition.gestureRecognition.currentGestureName);

					gestureSequenceRecognition.localizer.transform.position = gestureSequenceRecognition.maestroArm.playerPalm.position;
					gestureSequenceRecognition.localizer.transform.rotation = gestureSequenceRecognition.maestroArm.playerPalm.rotation;

					gestureSequenceRecognition.currentSequence.localPositionAtPoint.Add(gestureSequenceRecognition.localizer.transform.localPosition);
					gestureSequenceRecognition.currentSequence.localRotationAtPoint.Add(gestureSequenceRecognition.localizer.transform.localRotation);

					gestureSequenceRecognition.currentSequence.timeLimit.Add(2.0f);

					gestureSequenceRecognition.currentSequence.positionalLeniency.Add(1.0f);
					gestureSequenceRecognition.currentSequence.rotationalLeniency.Add(10.0f);

					recordingChapter++;
				}
			}

			if (GUILayout.Button("Save New Sequence")) {
				AssetDatabase.CreateAsset(currentSequence,"Assets/" + gestureSequenceRecognition.customSubdirectory + gestureSequenceRecognition.sequenceName + ".asset");

				gestureSequenceRecognition.StopRecordingSequence();
			}
		}
	}
}
