using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugLogger {

	public static void DebugMessage(GameObject sender,string message) {
		Debug.Log(sender.name + " - \"" + message + "\" - [" + Time.time + "]");
	}

	public static void DebugWarning(GameObject sender,string message) {
		Debug.LogWarning(sender.name + " - \"" + message + "\" - [" + Time.time + "]");
	}

	public static void DebugError(GameObject sender,string message) {
		Debug.LogError(sender.name + " - \"" + message + "\" - [" + Time.time + "]");
	}
}
