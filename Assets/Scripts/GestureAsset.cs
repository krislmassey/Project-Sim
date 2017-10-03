using System.Collections;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]
public class GestureAsset : ScriptableObject {

	public string gestureName;

	public bool skipRecognition;

	[HideInInspector]
	public bool createdWithLeftHand;

	public Vector3[] positions;

	public Quaternion[] rotations;

	[Range(0,90)]
	public float[] angleLeniencies;
}
