using UnityEngine;
using System.Collections;

public class MaestroArm : MonoBehaviour {

	public MaestroManager.ArmSide armSide;

	public Transform[] playerFingers;
	public Transform playerPalm;

	//public KnucklesPhysicalInteraction pickup;
	//public GestureRecognition gestureRecognizer;

	public Vector3 deltaHandDirection;
	public Vector3 deltaPalmPosition;

	public float deltaHandDistance;

	void Awake() {
		if (GetComponentInParent<MaestroManager>()) {
			if (armSide == MaestroManager.ArmSide.Left) {
				GetComponentInParent<MaestroManager>().leftArm = this;
			} else {
				GetComponentInParent<MaestroManager>().rightArm = this;
			}
		}
	}

	void Update() {
		//StoreDeltaPosition ();
		CalculateDeltas();
	}

	void StoreDeltaPosition() {
		deltaPalmPosition = playerPalm.position;
	}

	void CalculateDeltas() {
		deltaHandDirection = playerPalm.position - deltaPalmPosition;
		deltaPalmPosition = playerPalm.position;

		deltaHandDistance = Vector3.Distance(playerPalm.position,deltaPalmPosition);
	}

	public Vector3 GetHandDirection() {
		return playerPalm.position - deltaPalmPosition;
	}

	public float GetHandVelocity() {
		return Vector3.Distance (transform.position, deltaPalmPosition);
	}
}