using UnityEngine;
using System.Collections;
using Valve.VR;
using System;
using Maestro;

public class MaestroManager : MonoBehaviour {

	public static MaestroManager i;

	public enum ArmSide {Left,Right}
	public enum Finger {Thumb, Index, Middle, Ring, Little}

	public MaestroArm leftArm;
	public MaestroArm rightArm;

	public bool freezeTimeDuringCalibration;
	public bool inMenu;
	public bool hideMainRigWhileCalibrating;

	float timeScaleBookmark;

	public GameObject menuRigPrefab;
	public GameObject menuRig;

	void Awake() {
		i = this;

		AssignArms ();

		Invoke("DelayedDeviceActivation",1.0f);

		if (LayerMask.NameToLayer("Calibration") == -1) {
			Debug.LogError("There is no layer called \"Calibration\", which is required for the built-in Maestro menu. Please name layer 8 \"Calibration\" (without quotation marks) and start again.");
		}
	}

	void DelayedDeviceActivation() {
		//leftArm.transform.parent.gameObject.SetActive(true);
        rightArm.transform.parent.gameObject.SetActive(true);

        /*foreach (Collider c in GetComponentsInChildren<Collider>()) {
			c.isTrigger = true;
		}*/
        //rightArm.gameObject.SetActive(true);
    }

	void AssignArms() {
		MaestroArm[] arms = GetComponentsInChildren<MaestroArm> ();

		foreach (MaestroArm arm in arms) {
			if (arm.armSide == ArmSide.Left) {
				leftArm = arm;
			} else {
				rightArm = arm;
			}
		}
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.F9)) {
			if (!inMenu) {
				ActivateCalibrationSequence();
				menuRig.GetComponent<MaestroSettingsRig>().GetComponentInChildren<CalibrationSequence>().destroyRigWhenDone = true;
			} else {
				DestroyMenuRig();
			}
		}
		
		if (Input.GetKeyDown(KeyCode.F10)) {
			if (!inMenu) {
				ActivateMainSettings();
			} else {
				DestroyMenuRig();
			}
		}
	}

	public void ActivateMainSettings() {
		InstantiateMenuRig();

		if (menuRig) {
			menuRig.GetComponent<MaestroSettingsRig>().ToggleMainSettings(true);
		} else {
			Debug.Log("MaestroManager: The menuRig doesn't seem to exist yet, so I can't instruct it!");
		}
	}

	public void ActivateCalibrationSequence() {
		InstantiateMenuRig();

		if (menuRig) {
			menuRig.GetComponent<MaestroSettingsRig>().ToggleCalibrationSequence(true);
			//menuRig.GetComponentInChildren<CalibrationSequence>().Begin();
		} else {
			Debug.Log("MaestroManager: The menuRig doesn't seem to exist yet, so I can't instruct it!");
		}
	}

	public void InstantiateMenuRig() {
		if (!inMenu) {
            int leftIndex, rightIndex;
            bool leftActive, rightActive;
            GetArmSettings(leftArm, out leftIndex, out leftActive);
            GetArmSettings(rightArm, out rightIndex, out rightActive);

            inMenu = true;
			menuRig = Instantiate(menuRigPrefab, transform, false) as GameObject;
			menuRig.transform.SetParent (null);

            menuRig.SetActive(true);

            SetArmSettings<LeftMaestroGloveBehaviour>(leftIndex, leftActive);
            SetArmSettings<RightMaestroGloveBehaviour>(rightIndex, rightActive);

            MaestroSettingsRig menuSettings = menuRig.GetComponent<MaestroSettingsRig>();

			if (!hideMainRigWhileCalibrating) {
				menuSettings.DestroyListener();
			}

			menuSettings.callBackManager = this;

			if (freezeTimeDuringCalibration) {
				timeScaleBookmark = Time.timeScale;
				Time.timeScale = 0;
			}

			if (hideMainRigWhileCalibrating) {
				gameObject.SetActive (false);
			}
		}
	}

    private void GetArmSettings(MaestroArm arm, out int index, out bool active)
    {
        index = 0;
        active = false;
        
        SteamVR_TrackedObject[] trackedObjects = arm.GetComponentsInParent<SteamVR_TrackedObject>(true);
        if (trackedObjects.Length > 0)
        {
            index = (int)trackedObjects[0].index;
            active = index != 0 ? trackedObjects[0].gameObject.activeSelf : false;
        }
    }

    /*private void CopyArmSettings<T>(MaestroArm arm, out int index, out bool active) where T : MaestroGloveBehaviour
    {
        index = 0;
        active = false;

        var newArm = menuRig.GetComponentInChildren<T>(true);
        if (newArm)
        {
            SteamVR_TrackedObject[] trackedObjects = arm.GetComponentsInParent<SteamVR_TrackedObject>();
            if (trackedObjects.Length > 0)
            {
                //SteamVR_TrackedObject[] newTrackedObjects = newArm.GetComponentsInParent<SteamVR_TrackedObject>();
                //if (newTrackedObjects.Length > 0)
                //{
                index = (int)trackedObjects[0].index;
                //    newTrackedObjects[0].SetDeviceIndex((int)trackedObjects[0].index);
                //}
            }

            active = index != 0 ? arm.gameObject.activeSelf : false;

            //newArm.gameObject.SetActive(index != 0 ? arm.gameObject.activeSelf : false);
        }
    }*/

    private void SetArmSettings<T>(int index, bool active) where T : MaestroGloveBehaviour
    {
        var newArm = menuRig.GetComponentInChildren<T>(true);
        if (newArm)
        {
            SteamVR_TrackedObject[] trackedObjects = newArm.GetComponentsInParent<SteamVR_TrackedObject>();
            if (trackedObjects.Length > 0)
            {
                trackedObjects[0].SetDeviceIndex(index);
                trackedObjects[0].gameObject.SetActive(active);
            }

            //newArm.transform.parent.gameObject.SetActive(active);
        }
    }

    public void DestroyMenuRig() {
		if (inMenu) {
			Destroy (menuRig.gameObject);
			menuRig = null;

			if (freezeTimeDuringCalibration) {
				Time.timeScale = timeScaleBookmark;
			}

			inMenu = false;

            if (hideMainRigWhileCalibrating)
            {
                gameObject.SetActive(true);

                foreach (var what in gameObject.GetComponentsInChildren<SteamVR_TrackedObject>(true))
                {
                    if (what.index != SteamVR_TrackedObject.EIndex.None)
                    {
                        what.gameObject.SetActive(true);
                    }
                }
            }
        }
	}

	public void CloseMenuRig() {
		DestroyMenuRig();
	}
}