using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GestureRecognition))]

public class GestureRecognitionEditor : Editor {

	//MaestroArm maestroArm;
	GestureRecognition gestureRecognition;

	[Tooltip("Enables explanations for each feature.")]
	public bool showHelp;

	void OnEnable() {
		gestureRecognition = (GestureRecognition)target;
		//maestroArm = gestureRecognition.GetComponent<MaestroArm>();
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector();

			//if (gestureRecognition.helpBoxes) {
			//	EditorGUILayout.HelpBox("This will create a new gesture using the current shape of the hand and also save a .mgf file with these settings using the name found in the 'fileName' field.",MessageType.None);
			//}

			//if (GUILayout.Button("Create Gesture")) {
			//	gestureRecognition.CreateGesture();
			//}


		if (gestureRecognition.enableEditorMode) {
			showHelp = EditorGUILayout.Toggle("Show Help",showHelp);

			if (showHelp) {
				EditorGUILayout.HelpBox("This is the Maestro gesture recognition system. Before getting started, make sure your ContactCI Maestro glove is properly configured and ensure the MaestroArm component has a reference to each finger tip in the PlayerFingers array!",MessageType.None);
			}

			if (showHelp) {
				EditorGUILayout.HelpBox("To get started with creating a gesture, enter a name for it in the 'GestureName' field and hit play. When you're satisfied with the shape you're making, simply click the 'Create Gesture' button and the script will handle the rest.",MessageType.None);
			}

			//if (GUILayout.Button("Create Gesture")) {
			//	CreateGesture();
			//}

			if (GUILayout.Button("Create Gesture")) {
				gestureRecognition.CaptureGesture();
			}

			if (showHelp) {
				EditorGUILayout.HelpBox("The 'Create Gesture' button will create a new 'gesture object' using the current shape of the hand and store it in a 'GestureIndex' child object. It will also save it as a .asset file for later loading if need be. If no custom subdirectory is specified, it will be saved in the Assets folder. The name found in the 'fileName' field will be used to name the file, as well as in Gesture-related events and callbacks.",MessageType.None);
			}



			if (GUILayout.Button("Load Gesture")) {
				gestureRecognition.LoadGesture();
				//LoadGesture();
			}

			if (showHelp) {
				EditorGUILayout.HelpBox("The 'Load Gesture' button will load a gesture into the scene from a pre-saved .asset file and turn it into a 'gesture object' kept in the GestureIndex. If no custom subdirectory is specified, it will be looked for in the Assets folder.",MessageType.None);
			}

			//if (GUILayout.Button("Search for Gesture file")) {
			//	if (SearchForAsset(gestureRecognition.gestureName)) {
			//		Debug.Log("GestureRecognitionEditor: The Gesture asset [" + gestureRecognition.gestureName + "] was found.");
			//	} else {
			//		Debug.Log("GestureRecognitionEditor: The Gesture asset [" + gestureRecognition.gestureName + "] was not found.");
			//	}
			//}

			if (GUILayout.Button("Clear Gesture List")) {
				gestureRecognition.ClearAllGestures();
			}

			if (showHelp) {
				EditorGUILayout.HelpBox("The 'Clear Gesture List' button will clear the gesture list and delete all gesture objects from the GestureIndex.",MessageType.None);
			}

			if (showHelp) {
				EditorGUILayout.HelpBox("Further tips and tricks.\n+ If you want to temporarily deactivate or ignore a gesture at run-time, simply tick the 'Skip Recognition' bool on the 'gesture object' under the GestureIndex. This can be ticked by default in the saved .asset file.\n+ Each gesture also keeps a 'leniency' variable for each finger. A lower value means a stricter margin of error, while a higher value allows easier recognition for that digit. It can make a lot of difference.",MessageType.None);
			}
		}
	}

	//bool SearchForAsset(string assetName) {
	//	string[] results = AssetDatabase.FindAssets(assetName);

	//	foreach (string s in results) {
	//		Debug.Log(AssetDatabase.GUIDToAssetPath(s));
	//	}

	//	if (results.Length > 0) {
	//		return true;
	//	} else {
	//		return false;
	//	}
	//}

	//public void CreateGesture() {
	//	GestureAsset gesture = ScriptableObject.CreateInstance<GestureAsset>();

	//	gesture.gestureName = gestureRecognition.assetName;

	//	gesture.positions = new Vector3[5];
	//	gesture.rotations = new Quaternion[5];
	//	gesture.angleLeniencies = new float[5];

	//	Transform localizer = new GameObject().transform;
	//	localizer.SetParent(gestureRecognition.gestureIndex.transform);

	//	for (int f = 0; f < 5; f++) {
	//		localizer.position = maestroArm.playerFingers[f].position;
	//		localizer.rotation = maestroArm.playerFingers[f].rotation;

	//		gesture.positions[f] = localizer.localPosition;
	//		gesture.rotations[f] = localizer.localRotation;

	//		gesture.angleLeniencies[f] = 20.0f;
	//	}

	//	DestroyImmediate(localizer.gameObject);

	//	AssetDatabase.CreateAsset(gesture,"Assets/" + gestureRecognition.customSubdirectory + gestureRecognition.assetName + ".asset");

	//	gestureRecognition.LoadGestureFromAsset(gesture);
	//}

	//public void LoadGesture() {
	//	gestureRecognition.ManageGestureIndex();

	//	GestureAsset gesture = ScriptableObject.CreateInstance<GestureAsset>();
	//	string filePath = "";

	//	if (string.IsNullOrEmpty(gestureRecognition.customSubdirectory)) {
	//		filePath = "Assets/" + gestureRecognition.assetName + ".asset";
	//		gesture = (GestureAsset) AssetDatabase.LoadAssetAtPath(filePath,typeof(GestureAsset));
	//	} else {
	//		filePath = "Assets/" + gestureRecognition.customSubdirectory + gestureRecognition.assetName + ".asset";
	//		gesture = (GestureAsset) AssetDatabase.LoadAssetAtPath(filePath,typeof(GestureAsset));
	//	}

	//	if (gesture != null) {
	//		gestureRecognition.LoadGestureFromAsset(gesture);
	//	} else {
	//		Debug.LogError("GestureRecognitionEditor: Load failed using the filepath [" + filePath + "]. Most likely error is filepath-related, the gesture name is mistyped, or simply doesn't exist. If the expected location is a subdirectory of Assets, make sure to specify which in the 'subdirectory' field! e.g. Gestures/PaperScissorsRock/");
	//	}
	//}
}
