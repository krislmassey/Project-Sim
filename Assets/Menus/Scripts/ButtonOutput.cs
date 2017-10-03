using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class ButtonOutput : MonoBehaviour {

	public UnityEvent OnButtonPress;
	public UnityEvent OnButtonRelease;

	public Vector3 origin;
	Rigidbody rb;

	public Transform buttonTrans;

	public bool buttonDown;
	public bool On;
	public float buttonSensitivity;

	bool pressed;

	public float slideForce;

	void Awake() {

		origin = buttonTrans.localPosition;
		rb = buttonTrans.GetComponent<Rigidbody> ();
		rb.velocity = Vector3.zero;

	}

	void OnCollisionEnter(){

		pressed = true;

	}

	void OnCollisionExit() {

		pressed = false;

	}

	void Update () {
	
		float y = buttonTrans.localPosition.y;

		if (y < 0) {

			//rb.velocity += buttonTrans.up * slideForce;
			if(!pressed){

			 buttonTrans.Translate(buttonTrans.forward*0.01f);

			}

			if (y < -buttonSensitivity) {

				if (!buttonDown) {

					ButtonPushed ();

				}

			}

		} else {

			if (buttonDown) {

				ButtonReleased ();
			}

			//buttonTrans.localPosition = Vector3.zero;
			//rb.velocity = Vector3.zero;

		}

		buttonTrans.localPosition = new Vector3 (origin.x, Mathf.Clamp (buttonTrans.localPosition.y, -buttonSensitivity, 0), origin.z);

	}

	void ButtonPushed() {

		buttonDown = true;

		On = !On;

		Debug.Log("ButtonPress");

		OnButtonPress.Invoke();


	}

	void ButtonReleased() {

		Debug.Log("ButtonRelease");

		buttonDown = false;
		buttonTrans.localPosition = Vector3.zero;
		rb.velocity = Vector3.zero;

	}

}