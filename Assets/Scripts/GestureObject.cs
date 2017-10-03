using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GestureObject : MonoBehaviour {

	public string gestureName;
	public bool skipRecognition;

	[Range(0,90)]
	public float[] angleLeniencies;

	public List<Transform> gestureFingers = new List<Transform>(5);
}