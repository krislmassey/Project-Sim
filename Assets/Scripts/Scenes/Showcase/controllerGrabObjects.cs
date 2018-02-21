using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//source:  https://www.raywenderlich.com/149239/htc-vive-tutorial-unity

public class controllerGrabObjects : MonoBehaviour {
	private SteamVR_TrackedObject trackedObj;
	public GameObject collidingObject;
	public GameObject objectInHand;
	private SteamVR_Controller.Device Controller
	{
		get {return SteamVR_Controller.Input((int)trackedObj.index); }
	}

    public Shader regularShader;
    public Shader highlightShader;
    public Renderer rend;

    void Start()
    {
        regularShader = Shader.Find("Diffuse");
        highlightShader = Shader.Find("Outlined/Silhouetted Diffuse");
    }

    void Awake(){
		trackedObj = GetComponent<SteamVR_TrackedObject> ();
	}

	public GameObject GetObjectInHand(){
		return this.objectInHand;
	}

	private void SetCollidingObject(Collider col)
	{
		// 1
		if (collidingObject || !col.GetComponent<Rigidbody>())
		{
			return;
		}
		// 2
		collidingObject = col.gameObject;

        //get colliding Object renderer
        rend = collidingObject.GetComponent<Renderer>();
	}

	// 1
	public void OnTriggerEnter(Collider other)
	{
		SetCollidingObject(other);
	}

	// 2
	public void OnTriggerStay(Collider other)
	{
		SetCollidingObject(other);
	}

	// 3
	public void OnTriggerExit(Collider other)
	{
		if (!collidingObject)
		{
			return;
		}

		collidingObject = null;
	}

	private void GrabObject()
	{
		// 1
		objectInHand = collidingObject;
		collidingObject = null;
		// 2
		var joint = AddFixedJoint();
		joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
	}

	// 3
	private FixedJoint AddFixedJoint()
	{
		FixedJoint fx = gameObject.AddComponent<FixedJoint>();
		fx.breakForce = 20000;
		fx.breakTorque = 20000;
		return fx;
	}

	private void ReleaseObject()
	{
		// 1
		if (GetComponent<FixedJoint>())
		{
			// 2
			GetComponent<FixedJoint>().connectedBody = null;
			Destroy(GetComponent<FixedJoint>());
			// 3
			objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
			objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
		}
		// 4
		objectInHand = null;
	}

	// Update is called once per frame
	void Update () {
		// 1
		if (Controller.GetHairTriggerDown())
		{
			if (collidingObject)
			{
				GrabObject();
                //debug output object information
                Debug.Log("Object Selected: " + collidingObject.name);

                //highlight the colliding object
                rend.material.shader = highlightShader;

                //set colliding object status to "selected"?
			}
		}

		// 2
		if (Controller.GetHairTriggerUp())
		{
			if (objectInHand)
			{
				ReleaseObject();
                rend.material.shader = regularShader;
			}
		}	
	}
}
