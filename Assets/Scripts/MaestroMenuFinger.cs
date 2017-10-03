using UnityEngine;
using System.Collections.Generic;

public class MaestroMenuFinger : MonoBehaviour {

	/*---------------------------------------------------------------------------------------------------------------

	+ Guide / Troubleshooting for the VRButton system

	+ The three main variables below (fingerRadius, pushSpeed and pushDepth) must not be 0 and have suggested sizes by default.

	+ For a VRButton to be valid, it must have:
		- The VRButton component.
		- A collider of any kind, set as a trigger.
		- Be on a layer called 'Calibration'.
		- Be childed at 0,0,0 to a GameObject, which will act as the anchor.

	+ For a VRButtonFinger to be valid, it must have:
		- This script (Of course...)
		- A collider of any kind, trigger does not matter. The radius should roughly match that set to fingerRadius, adjusting for any un-normalized scales.
		- It's also preferred that this object itself not be set to the Calibration layer for optimization reasons, if possible, although it is not crucial.
		- That's it!

	+ Please ensure your VRButtons don't have a Rigidbody attached to them! The parent/anchor may have one, but not the same object as the VRButton itself.

	+ The button depth and speed is shared between all buttons for simplicitys sake. If this is unsuitable, hack away!

	 --------------------------------------------------------------------------------------------------------------*/

	int layerMask;

	Collider myCol;

	[HideInInspector]
	List<Collider> buttonColliders;

	public List<MaestroMenuButton> touchedButtons;

	[HideInInspector]
	Collider[] cols;

	public bool drawGizmos;

	public float fingerRadius = 0.01f; // This is the radius of the fingers influence.

	public float pushSpeed = 0.0005f; // This is the rate at which the button will move backwards and forwards when collisions with a finger are detected.

	public float pushDepth = 0.1f; // This is the maximum local z depth that the button can be pushed back to.

	void Start() {
		layerMask = 1 << LayerMask.NameToLayer("Calibration");
		myCol = GetComponent<Collider>();
		buttonColliders = new List<Collider>();
		touchedButtons = new List<MaestroMenuButton>();
		cols = new Collider[4];
	}

	void Update() {
		GetColliders();
		HandleColliders();
	}

	void OnDrawGizmos() {
		if (drawGizmos) {
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(transform.position,fingerRadius);
		}
	}

	void GetColliders() {
		Physics.OverlapSphereNonAlloc(transform.position,fingerRadius, cols, layerMask); // Get all the colliders within a designated radius...

		foreach (Collider c in cols) { // If each one...
			if (c != null) {
				if (!buttonColliders.Contains(c)) { // ...isn't already on the list...
					if (c.GetComponent<MaestroMenuButton>()) { // ...has the VRButton component...
						c.GetComponent<MaestroMenuButton>().OnButtonTouch.Invoke(); // ...trigger the 'touch' event...
						buttonColliders.Add(c); // ...add it to the list of colliders...
						touchedButtons.Add(c.gameObject.GetComponent<MaestroMenuButton>()); // ...and also to the list of VRButton components...

						while (myCol.bounds.Intersects(c.bounds)) { // ...Also, while this collider is touching it...
							MoveButton(c,pushSpeed); // ...keep moving it forward a bit.

							if (c.transform.localPosition.z >= pushDepth) { // If the button is now past the maximum allowed push amount...
								SetButtonPosition(c,pushDepth); // ...Lock it in place...

								c.GetComponent<MaestroMenuButton>().OnButtonPush.Invoke(); // ...and finally, trigger the 'push' event for it! Though getting this far in one frame is very unlikely.
								return;
							}
						}
					}
				}
			}
		}
	}

	void HandleColliders() {
		for (int i = 0; i < buttonColliders.Count; i++) { // With each button...
			if (buttonColliders[i].transform.localPosition.z == pushDepth) { // If the button is already at the maximum allowed distance...
				if (!myCol.bounds.Intersects(buttonColliders[i].bounds)) { // ...but this finger isn't still touching it...
					touchedButtons[i].OnButtonRelease.Invoke(); // It must have been released...
					MoveButton(i,-pushSpeed * 2.0f); // And we can move it back a bit...
				}
			} else if (buttonColliders[i].transform.localPosition.z >= 0) { // If the button isn't all the way pushed...
				if (!myCol.bounds.Intersects(buttonColliders[i].bounds)) { // And its not still being touched...
					MoveButton(i,-pushSpeed * 2.0f); // Move it back a bit...

					if (buttonColliders[i].transform.localPosition.z <= 0) { // And if that was enough to be back at 0.
						SetButtonPosition(i,0); // Set that position!

						//touchedButtons[i].OnButtonRelease.Invoke(); // And trigger the 'release' event...

						buttonColliders.RemoveAt(i); // Remove it!
						touchedButtons.RemoveAt(i);
						return;
					}
				} else { // If the button isn't all the way pushed...
					while (myCol.bounds.Intersects(buttonColliders[i].bounds)) { // While its touching this finger...
						MoveButton(i,pushSpeed); // Push it forward continuously...

						if (buttonColliders[i].transform.localPosition.z >= pushDepth) { // If the button is now past the maximum allowed push amount...
							SetButtonPosition(i,pushDepth);  // Snap it to that amount...

							touchedButtons[i].OnButtonPush.Invoke(); // And trigger the 'push' event for it!
							return;
						}
					}
				}
			}
		}
	}

	void MoveButton(int bID,float amount) {
		buttonColliders[bID].transform.Translate(new Vector3(0,0,amount),Space.Self);
	}

	void MoveButton(Collider c,float amount) {
		c.transform.Translate(new Vector3(0,0,amount),Space.Self);
	}

	void SetButtonPosition(int bID,float newDepth) {
		buttonColliders[bID].transform.localPosition = new Vector3(0,0,newDepth); // Snap it to that amount...
	}

	void SetButtonPosition(Collider c,float newDepth) {
		c.transform.localPosition = new Vector3(0,0,newDepth); // Snap it to that amount...
	}
}