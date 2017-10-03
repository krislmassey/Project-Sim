using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class GestureEvent : UnityEvent<GestureObject> {}

[RequireComponent(typeof(MaestroArm))]
public class GestureRecognition : MonoBehaviour {

	//----------------------------------------------------------------------------------------------
	// References
	//----------------------------------------------------------------------------------------------

	[HideInInspector]
	public GameObject gestureIndex;

	[Tooltip("This is the field used for specifying which gesture file to load or save by name. No file extension is required.")]
	public string assetName;

	[Tooltip("If a directory is specified, this is where gesture files will be saved to and loaded from. If no directory is specified, it will default to the Assets folder.")]
	public string customSubdirectory;

	[HideInInspector]
	private MaestroArm maestroArm;

	[HideInInspector]
	public string fileExtension;

	[Tooltip("This activates the creative options for saving and loading gestures.")]
	public bool enableEditorMode;

	//[Tooltip("Populate this array with each finger tip of your hand object and ensure that the order chosen remains consistent between gesture files and projects.")]
	[HideInInspector]
	public Transform[] playerFingers;
	
	[Tooltip("When true, recognition will be deactivated.")]
	public bool skipRecognition;
	
	//[Tooltip("This will be true if a gesture is currently recognised.")]
	//[HideInInspector]
	public bool gestureComplete;

	public GameObject gestureFingertipPrefab;
	
	bool keepGestureChecking;
	
	public int fingerSuccessCount;
	
	public string currentGestureName;

	public bool debugRays;

	//[HideInInspector]
	public int currentGestureID;
	
	//[HideInInspector]
	public GestureEvent OnGestureBegin;

	//[HideInInspector]
	public GestureEvent OnGestureEnd;
	
	//[Tooltip("This is the list of gestures that will be attempted to be recognised at run time.")]
	//[HideInInspector]
	
	public List<GestureObject> gestures;

	//----------------------------------------------------------------------------------------------
	// Initialization
	//----------------------------------------------------------------------------------------------
	
	void OnEnable() {
		Initialize();
	}

	public void Initialize() {
		maestroArm = GetComponent<MaestroArm>();
		//maestroArm.gestureRecognizer = this;
		playerFingers = maestroArm.playerFingers;
		
		if (OnGestureBegin == null)
			OnGestureBegin = new GestureEvent();

		if (OnGestureEnd == null)
			OnGestureEnd = new GestureEvent();

		ManageGestureIndex();
	}
	
	//----------------------------------------------------------------------------------------------
	// Asset saving/loading/management
	//----------------------------------------------------------------------------------------------
	
	public void ManageGestureIndex() {
		if (gestureIndex) {
			//return true;
		} else if (transform.FindChild("GestureIndex")) {
			gestureIndex = transform.FindChild("GestureIndex").gameObject;
			//return true;
		} else if (GetComponent<MaestroArm>()) {
			if (GetComponent<MaestroArm>().playerPalm) {
				gestureIndex = new GameObject("GestureIndex");
				gestureIndex.transform.SetParent(transform);
				gestureIndex.transform.localPosition = Vector3.zero;
				gestureIndex.transform.localRotation = new Quaternion(0,0,0,0);
				gestureIndex.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
				//return true;
			} else {
				Debug.LogWarning("GestureRecognition: The 'playerPalm' property of MaestroArm has not yet been assigned. Gestures are local to the users hand.");
				//return false;
			}
		} else {
			Debug.LogWarning("GestureRecognition: There is no 'MaestroArm' component currently attached to this GameObject.");
			//return false;
		}
	}

	public void LoadGesture() {
		GestureAsset gesture = ScriptableObject.CreateInstance<GestureAsset>();
		string filePath = "";

		filePath = "Assets/" + customSubdirectory + assetName + ".asset";
		gesture = (GestureAsset) AssetDatabase.LoadAssetAtPath(filePath,typeof(GestureAsset));

		if (gesture != null) {
			ManageGestureIndex();

			if (gestureIndex.transform.FindChild("\"" + assetName + "\"")) {
				Debug.LogError("GestureRecognition: The specified gesture \"" + assetName + "\" already exists in the gesture index.");
				return;
			} else {

				GameObject newGestureObject = new GameObject();
				newGestureObject.transform.SetParent(gestureIndex.transform);
				newGestureObject.transform.localPosition = Vector3.zero;
				newGestureObject.transform.localRotation = new Quaternion(0,0,0,0);
				newGestureObject.transform.localScale = new Vector3(1,1,1);
				newGestureObject.name = "\"" + assetName + "\"";

				GestureObject newGesture = newGestureObject.AddComponent<GestureObject>();
				newGesture.gestureName = gesture.gestureName;
				newGesture.skipRecognition = gesture.skipRecognition;

				newGesture.angleLeniencies = new float[5];

				for (int f = 0; f < 5; f++) {
					GameObject currentFinger = new GameObject();
					currentFinger.transform.SetParent(newGestureObject.transform);
					currentFinger.transform.localScale = new Vector3(1,1,1);

					newGesture.gestureFingers.Add(currentFinger.transform);
					newGesture.angleLeniencies[f] = gesture.angleLeniencies[f];

					currentFinger.transform.localPosition = gesture.positions[f];
					currentFinger.transform.localRotation = gesture.rotations[f];

					GameObject tipMesh = (GameObject) Resources.Load("GestureFingertip",typeof(GameObject));
					GameObject newTipMesh = Instantiate(tipMesh,currentFinger.transform,false) as GameObject;

					newTipMesh.transform.localEulerAngles = new Vector3(90.0f,0.0f,0.0f);
                    newTipMesh.SetActive(false);

					switch (f) {
					case 0:
						currentFinger.name = "Thumb";
						break;
					case 1:
						currentFinger.name = "Index";
						break;
					case 2:
						currentFinger.name = "Middle";
						break;
					case 3:
						currentFinger.name = "Ring";
						break;
					case 4:
						currentFinger.name = "Little";
						break;
					}
				}
				gestures.Add(newGesture);

				//Debug.Log(maestroArm);
				//Debug.Log(MaestroManager.ArmSide.Left);
				//Debug.Log(gesture.createdWithLeftHand);

				if (!maestroArm) {
					maestroArm = GetComponent<MaestroArm>();
				}

				// If the side of this arm doesn't match the side that the gesture was created with, we flip the GestureObjects local x scale so that the tips match up once more.
				if ((maestroArm.armSide == MaestroManager.ArmSide.Left && !gesture.createdWithLeftHand) || (maestroArm.armSide == MaestroManager.ArmSide.Right && gesture.createdWithLeftHand)) {
					newGestureObject.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
				}
			}
		} else {
			Debug.LogError("GestureRecognitionEditor: Load failed using the filepath [" + filePath + "]. Most likely error is filepath-related, the gesture name is mistyped, or simply doesn't exist. If the expected location is a subdirectory of Assets, make sure to specify which in the 'subdirectory' field! e.g. Gestures/PaperScissorsRock/");
		}
	}

	public void CaptureGesture() {
		#if UNITY_EDITOR
		ManageGestureIndex();

		GestureAsset gesture = ScriptableObject.CreateInstance<GestureAsset>();

		gesture.positions = new Vector3[5];
		gesture.rotations = new Quaternion[5];
		gesture.angleLeniencies = new float[5];

		if (!maestroArm) {
			maestroArm = GetComponent<MaestroArm>();
		}

		if (maestroArm.armSide == MaestroManager.ArmSide.Left) {
			gesture.createdWithLeftHand = true;
		} else {
			gesture.createdWithLeftHand = false;
		}

		Transform localizer = new GameObject().transform;
		localizer.name = "Localizer";
		localizer.SetParent(gestureIndex.transform);

		for (int f = 0; f < 5; f++) {
			localizer.position = maestroArm.playerFingers[f].position;
			localizer.rotation = maestroArm.playerFingers[f].rotation;

			gesture.positions[f] = localizer.localPosition;
			gesture.rotations[f] = localizer.localRotation;

			gesture.angleLeniencies[f] = 20.0f;
		}

		if (Application.isPlaying) {
			Destroy(localizer.gameObject);
		} else {
			DestroyImmediate(localizer.gameObject);
		}

		if (string.IsNullOrEmpty(assetName)) { // If we don't have a pre-assigned asset name to use...
			int newGestureID = 0;

			// We count to 100 and each time, check if a gesture called "New[Left/Right]Gesture[number]" doesn't already exist.

			if (maestroArm.armSide == MaestroManager.ArmSide.Left) {
				while (AssetDatabase.FindAssets("NewLeftGesture" + newGestureID.ToString()).Length != 0 && newGestureID < 100) {
					++newGestureID;
				}
			} else {
				while (AssetDatabase.FindAssets("NewRightGesture" + newGestureID.ToString()).Length != 0 && newGestureID < 100) {
					++newGestureID;
				}
			}

			
			if (maestroArm.armSide == MaestroManager.ArmSide.Left) {
				assetName = "NewLeftGesture" + newGestureID.ToString();
			} else {
				assetName = "NewRightGesture" + newGestureID.ToString();
			}

			if (newGestureID >= 100) { // We stop operations if we got to 100, thats too many! 
				return;
			}
		}

		gesture.gestureName = assetName;

		AssetDatabase.CreateAsset(gesture,"Assets/" + customSubdirectory + assetName + ".asset");

		//LoadGesture();

		assetName = "";
		#else
			Debug.LogWarning("A new gesture asset cannot be created at this time because this is happening outside of the editor.");
		#endif
	}

	public void ClearAllGestures() {
		foreach (GestureObject g in gestures) {
			if (g != null) {
				DestroyImmediate(g.gameObject);
			}
		}

		for (int i = 0; i < gestureIndex.transform.childCount; i++) {
			DestroyImmediate(gestureIndex.transform.GetChild(i).gameObject);
		}

		gestures.Clear ();
	}
	
	//----------------------------------------------------------------------------------------------
	// Update
	//----------------------------------------------------------------------------------------------
	
	void FixedUpdate() {
		HandleRecognition();

		//if (debugRays) {
		//	for (int i = 0; i < 5; i++) {
		//		Debug.DrawRay(gestures[0].gestureFingers[i].position,gestures[0].gestureFingers[i].forward * 0.1f,Color.black);

		//		if (Vector3.Angle(playerFingers[i].forward,gestures[0].gestureFingers[i].forward) < gestures[0].angleLeniencies[i]) {
		//			Debug.DrawRay(playerFingers[i].position,playerFingers[i].forward * 0.1f,Color.green);
		//		} else {
		//			Debug.DrawRay(playerFingers[i].position,playerFingers[i].forward * 0.1f,Color.red);
		//		}
		//	}
		//}
	}

	//void OnDrawGizmos() {
	//	if (gestures.Count > 0) {
	//		Gizmos.color = Color.black;
	//		GameObject g = (GameObject) Resources.Load("GestureFingertip",typeof(GameObject));
	//		Mesh m = g.GetComponent<MeshFilter>().sharedMesh;

	//		foreach (GestureObject gO in gestures) {
	//			foreach (Transform t in gO.gestureFingers) {
	//				Gizmos.DrawWireMesh(m,t.position,t.rotation);
	//			}
	//		}
	//	}
	//}
	
	//----------------------------------------------------------------------------------------------
	// Gesture recognition
	//----------------------------------------------------------------------------------------------
	
	void HandleRecognition() {
		if (skipRecognition) {
			return;
		} else {
			if (gestureComplete) { // If the "GestureComplete" bool is already true from last frame...
				CheckGesture (currentGestureID); // We should check that gesture first.

				if (!gestureComplete) { // If GestureComplete is now false...
					CheckAllButOneGesture (currentGestureID); // We should check the rest of the gestures just in case this game is being played by The Flash
				}
			} else { // Otherwise, simply check all possibly gestures
				CheckAllGestures ();
			}
		}
	}
	
	void CheckGesture(int gID) {
		if (gestures[gID] == null) {
			GestureUnrecognised();
			return;
		}

		if (!gestures [gID].skipRecognition) { // If we aren't supposed to skip this gesture...
			fingerSuccessCount = 0;

			for (int fID = 0; fID < 5; fID++) { // For each finger...
				if (debugRays) {
					Debug.DrawRay(gestures[gID].gestureFingers[fID].position,gestures[gID].gestureFingers[fID].forward * 0.05f,Color.black);
				}

				float angle = Vector3.Angle (playerFingers[fID].forward, gestures[gID].gestureFingers[fID].forward); // Check the angle between the users hand and the gestures 'finger direction'

				if (angle < gestures[gID].angleLeniencies[fID]) { // If the angle is lower than the allowed threshold (found in the asset itself)...
					fingerSuccessCount++;
					if (debugRays) {
						Debug.DrawRay(playerFingers[fID].position,playerFingers[fID].forward * 0.05f,Color.green);
					}
				} else {
					if (debugRays) {
						Debug.DrawRay(playerFingers[fID].position,playerFingers[fID].forward * 0.05f,Color.red);
					}

					fID = 5; // Quick way of ending the for loop.
				}
			}

			if (fingerSuccessCount == 5) {
				keepGestureChecking = false;

				//if (!gestureComplete) {
				if (gID != currentGestureID) {
					GestureRecognised (gID);
				}
			} else {
				if (gID != -1) {
					GestureUnrecognised();
				}
			}
		} else {
			//Debug.LogWarning ("This particular gesture is currently not active. If this is in error, please untick the 'skipRecognition' bool on this particular gesture.");

			if (gID == currentGestureID) { //If the gestureID to check has suddenly become skippable mid-game but the gesture is currently making that gesture, switch to 'Unrecognised'. This allows gestures to be stopped even if its currently being made by the player.
				//GestureRecognised (-1);
				GestureUnrecognised();
			}
		}
	}
	
	// The default checking routine when there's no current gesture.
	void CheckAllGestures() {
		keepGestureChecking = true;

		for (int gID = 0; gID < gestures.Count; gID++) {
			if (keepGestureChecking) {
				CheckGesture (gID);
			}
		}
	}

	// This exists as part of an optimization pattern.
	void CheckAllButOneGesture(int gIDToSkip) {
		keepGestureChecking = true;

		for (int gID = 0; gID < gestures.Count; gID++) {
			if (keepGestureChecking) {
				if (gID != gIDToSkip) {
					CheckGesture (gID);
				}
			}
		}
	}

	// This function gets called the moment a gesture is completely recognised. It invokes the OnGestureBegin event.
	public void GestureRecognised(int gID) {
		gestureComplete = true;
		currentGestureID = gID;
		currentGestureName = gestures[gID].gestureName;
		OnGestureBegin.Invoke(gestures[gID]);
	}

	// This function gets called the moment a gesture can no longer be recognised.
	// It invokes the OnGestureEnd event just before returning all values to 'Unrecognised', so scripts can react to the conditions of a gesture ending if need be.
	public void GestureUnrecognised() {
		if (currentGestureID != -1) {
			OnGestureEnd.Invoke(gestures[currentGestureID]);
		}

		gestureComplete = false;
		currentGestureID = -1;
		currentGestureName = "Unrecognised";
	}
}