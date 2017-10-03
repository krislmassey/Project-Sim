using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GestureCaptureTrigger : MonoBehaviour {

	float timer = 3.0f;
	public Text timeDisplay;

	public AudioClip cameraClick;
	public Image cameraIcon;
	public Color red;

	public UnityEvent OnComplete;


	void OnEnable() {
		if (GetComponentInParent<MaestroSettingsRig>().menuState != MaestroSettingsRig.MenuState.Main) {
			gameObject.SetActive(false);
			return;
		} else {
			StartCoroutine("CountDownToCapture");
		}
	}

	IEnumerator CountDownToCapture() {
		timer = 3.0f;

		cameraIcon.color = Color.white;

		GetComponent<Animator>().SetTrigger("Enable");

		while (timer > 0.0f) {
			timer -= Time.unscaledDeltaTime;
			timeDisplay.text = "Gesture Captured in " + Mathf.CeilToInt(timer);
			yield return null;
		}

		timeDisplay.text = "Say Cheese";

		//OnComplete.Invoke();
		TriggerCapture();

		AudioSource.PlayClipAtPoint(cameraClick,this.transform.position);
		cameraIcon.color = red;

		timer = 2.0f;

		while (timer > 0.0f) {
			timer -= Time.unscaledDeltaTime;
			yield return null;
		}

		GetComponentInParent<MaestroSettingsRig>().menuState = MaestroSettingsRig.MenuState.Main;

		GetComponent<Animator>().SetTrigger("Disable");

		gameObject.SetActive(false);

		yield return null;
	}

	void TriggerCapture() {
		OnComplete.Invoke();
	}
}
