﻿//=============================================================================
//
// Purpose: Keeps track of what this fingertip is touching. This script is added automatically; do not place it in your scene.
//
//=============================================================================

using UnityEngine;
using System.Collections;
using Maestro.Phalanges;

public class FingerTipCollider : MonoBehaviour {
	
	public MaestroPhysicalInteraction hpi; // the parent hand's interaction script.

	public Rigidbody rb; // my rigidbody

	MaestroInteractable touchtemp;

	public MaestroInteractable touching; // the thing I'm touching currently

	public Renderer rend; // the debug renderer for the collider. Is disabled by default.

	public Vector3 colnrm; // the normal of the current collision

	public int index; // which finger am I?

	float timer; //time since I touched something

	[HideInInspector]
	public Collider col; //keep track of my collider so I can turn it off in certain situations

	void Awake(){
		rb = GetComponent<Rigidbody> ();
	}

	void OnCollisionExit(Collision c){
		if (isvalid(c.collider)) {
			touching = null;
		}
	}

	void OnCollisionEnter(Collision c){
		
		if (isvalid (c.collider)) {
			hpi.hc.TriggerHaptics (600);
			timer = 0;
			//time to set the clamp value. The finger just touched the thing we're holding, so we don't want it to bend more.
			//index < 5 because index = 5 means this is the palm. Which doesn't bend.
			if (index < 5 && hpi.grabTarget == c.rigidbody) {
				hpi.hc.SetClamp (index, hpi.hc.getposfromindex (index));
			}
			c.rigidbody.GetComponent<MaestroInteractable> ().Touch (this); // tell the Interactable it got touched by this finger
		}
	}

	void Update(){
		//update the public touching variable and reset touchtemp so it has to be set again by OnCollisionStay during the next frame
		touching = touchtemp;
		if (touchtemp != null) {
			touchtemp = null;
		}

		//this mess just resets the finger clamping after 0.2 seconds of not touching anything
		if (touching) {
			timer = 0;
		} else {
			timer += Time.deltaTime;
			if (index < 5&&timer>0.2f) {
				hpi.hc.SetClamp (index, 0);
			}
		}
	}

	//this makes our collider, plus a renderer in case we want to debug collisions.
	public void makeRend(float radius){
		GameObject g = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		col = g.GetComponent<Collider>();

		g.transform.parent = transform;
		g.transform.localPosition = Vector3.zero;
		g.transform.localRotation = Quaternion.identity;
		g.transform.localScale = radius * Vector3.one * 2;
		g.GetComponent<Renderer> ().material = (Material)Resources.Load ("colliderDebug", typeof(Material));
		rend = g.GetComponent<Renderer> ();

        //Ignore all collisions with the hand itself
        foreach (Collider c in hpi.GetComponentsInChildren<Collider>())
        {
            Physics.IgnoreCollision(g.GetComponent<Collider>(), c);
        }
	}

	public void makeRend(Vector3 size){
		GameObject g = GameObject.CreatePrimitive (PrimitiveType.Cube);
		col = g.GetComponent<Collider>();
		g.transform.parent = transform;
		g.transform.localPosition = Vector3.zero;
		g.transform.localRotation = Quaternion.identity;
		g.transform.localScale = size;
		g.GetComponent<Renderer> ().material = (Material)Resources.Load ("colliderDebug", typeof(Material));
		rend = g.GetComponent<Renderer> ();

        foreach (Collider c in hpi.GetComponentsInChildren<Collider>())
        {
            Physics.IgnoreCollision(g.GetComponent<Collider>(), c);
        }
    }

	//check whether a collider is knuckles Interactable
	bool isvalid (Collider c){
		if (c.attachedRigidbody == null) {
			return false;
		} else {
			return (c.attachedRigidbody.GetComponent<MaestroInteractable> () != null && c.GetComponentInParent<FingerTipCollider> () == null);
		}
	}

	void OnCollisionStay(Collision c){
		if (isvalid(c.collider)) {
			colnrm = c.contacts [0].normal;
			if (c.collider.attachedRigidbody.GetComponent<MaestroInteractable> ().isGrabbable) { // make sure we're actually allowed to pick this up
				touchtemp = c.collider.attachedRigidbody.GetComponent<MaestroInteractable> ();// say what we're touching currently
			}
		}
	}
}
