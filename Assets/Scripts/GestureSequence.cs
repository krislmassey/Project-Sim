using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
[System.Serializable]
public class GestureSequence : ScriptableObject {

	public string sequenceName;

	public List<bool> considerTime;
	public List<bool> considerGesture;
	public List<bool> considerPosition;
	public List<bool> considerRotation;

	public List<string> gestureAtPoint;

	public List<Vector3> localPositionAtPoint;
	public List<Quaternion> localRotationAtPoint;

	public List<float> timeLimit;
	public List<float> positionalLeniency;
	public List<float> rotationalLeniency;
}