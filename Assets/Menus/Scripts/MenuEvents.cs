using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuEvents : MonoBehaviour {

	public Animator optionsMenu;
	public Animator startButton;
	public Animator backButton;


	public void Update () {

		/*if (Input.GetKeyDown(KeyCode.Space)) {

			DisableOptions();

		}

		if (Input.GetKeyDown(KeyCode.W)) {

			EnableStart();

		}

		if (Input.GetKeyDown(KeyCode.Q)) {

			DisableStart();

		}*/

	}


	public void EnableStart() {
		//back and start always get enabled together
		startButton.SetTrigger("Enable");
		backButton.SetTrigger("Enable");
		optionsMenu.SetTrigger("Enable");
		//Debug.Log("start triggered");

	}
	public void DisableStart(){

		startButton.SetTrigger("Disable");
		backButton.SetTrigger("Disable");

	}

	public void DisableEverything(){
		startButton.SetTrigger("Disable");
		backButton.SetTrigger("Disable");
		optionsMenu.SetTrigger("Disable");

	}

	public void DisableSettings() {

		startButton.SetTrigger("Disable");
		optionsMenu.SetTrigger("Disable");

	}

	public void BackButton() {

	}

	public void EnableOptions() {

		optionsMenu.SetTrigger("Enable");

	}

	public void DisableOptions() {

		optionsMenu.SetTrigger("Disable");

	}

	public void Calibrate(){

		optionsMenu.SetTrigger("Disable");

		startButton.SetTrigger("Disable");


	}

	public void Capture() {

		optionsMenu.SetTrigger("Disable");

		startButton.SetTrigger("Disable");

	}

}
