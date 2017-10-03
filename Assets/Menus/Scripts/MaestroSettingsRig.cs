using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maestro;

public class MaestroSettingsRig : MonoBehaviour {

	//[HideInInspector]
	public GameObject mainSettingsMenu;

	//[HideInInspector]
	public GameObject calibrationSequence;

	public GameObject GestureCapture;

	//[HideInInspector]
	//public GameObject audioListener;

	//[HideInInspector]
	public MaestroManager callBackManager;

	public MenuEvents settingsMenu;

	public enum MenuState { Main, Calibration, GestureCapture }
	public MenuState menuState;

	public void ToggleMainSettings(bool newActiveState) {

		if (mainSettingsMenu) mainSettingsMenu.SetActive(newActiveState);

		if(newActiveState){ //if settings menu getting turned on...
			
			//Debug.Log("Enabling");
					
			settingsMenu.EnableStart(); //this function turns on the start button (it contains an animation event)
			menuState = MenuState.Main;
		}

	}

	public void ToggleCalibrationSequence(bool newActiveState) {
        if (calibrationSequence)
        {
            var sequence = calibrationSequence.GetComponent<CalibrationSequence>();
            sequence.leftGlove = gameObject.GetComponentInChildren<LeftMaestroGloveBehaviour>();
            sequence.rightGlove = gameObject.GetComponentInChildren<RightMaestroGloveBehaviour>();
            calibrationSequence.SetActive(newActiveState);
        }
	}

	public void ToggleGestureCapture(bool newActiveState) {
		if (GestureCapture) GestureCapture.SetActive(newActiveState);
	}

	//functions for settings buttons

	public void CalibrationSelected() {
		ToggleCalibrationSequence(true);
		settingsMenu.DisableSettings();//this function sends disable animations to all components
	}

	public void GestureCaptureSelected() {
		ToggleGestureCapture(true);
	}

	public void DisableMainSettings(){
		ToggleMainSettings(false);
	}

	public void DestroyListener() {
		DebugLogger.DebugMessage(this.gameObject,"Deleting the AudioListener from the MenuRig");
		Destroy(GetComponentInChildren<AudioListener>().gameObject);
	}

	public void ExitMenu() {
        callBackManager.DestroyMenuRig();

        if (!callBackManager.gameObject.activeSelf) {

			callBackManager.gameObject.SetActive(true);
		}
	} 

	public void CalibrationCompleted() {
		//if (!callBackManager.gameObject.activeSelf) {

		//	callBackManager.gameObject.SetActive(true);

		//}

		//callBackManager.DestroyMenuRig();

		ToggleCalibrationSequence(false);
		ToggleMainSettings(true);
		ChangeMenuState("Main");

	}

	public void ChangeMenuState(string newState) {
		switch (newState) {
			case "Main":
				menuState = MenuState.Main;
				break;
			case "Calibration":
				menuState = MenuState.Calibration;
				break;
			case "GestureCapture":
				menuState = MenuState.GestureCapture;
				break;
		}
	}
}