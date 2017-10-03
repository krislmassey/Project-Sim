using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugThrowReset : MonoBehaviour {

	[HideInInspector]
	public Vector3 startPos;

	void Start() {
		startPos = transform.position;
	}

	public void OnLetGo() {
		Invoke("ResetPosition",2.5f);
	}

	void ResetPosition() {
		transform.position = startPos;
		transform.eulerAngles = Vector3.zero;

		Rigidbody rb = GetComponent<Rigidbody>();

		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
	}
}
