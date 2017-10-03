using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Maestro;

public class CalibrationSequence : MonoBehaviour {

	public enum CalibrationMode {Left,Right}
	public CalibrationMode calibrationMode;

	public int calibrationPhase;

	/*
	 * Phase 0 - Start
	 * Phase 1 - 'Recording' window for wrist calibration activated
	 * Phase 2 - 'Recording' window for finger proximals activated
	 * Phase 3 - 'Recording' window for finger distals activated
	 * Phase 4 - 'Completed' window activated
	 */

	public Animator handSelection;
	public Animator handCalibration;
	public Animator handComplete;
	public Animator guideHand;

	public AudioClip phase1;
	public AudioClip phase2;
	public AudioClip phase3;

	public AudioClip hoverStart;
	public AudioClip hoverComplete;
	public AudioClip complete1;
	public AudioClip complete2;

	public Text calibrationTitle;

	public AudioSource menuSpeaker1;
	public AudioSource menuSpeaker2;
	public AudioSource menuSpeaker3;
	
	public bool destroyRigWhenDone;

    public LeftMaestroGloveBehaviour leftGlove;
    public RightMaestroGloveBehaviour rightGlove;

    void OnEnable() {
		//GazeRay.OnGazeBegin += PlayHoverStartSFX;
	}

	void OnDisable() {
		//GazeRay.OnGazeBegin -= PlayHoverStartSFX;
	}

	public void PlayHoverStartSFX() {
		menuSpeaker3.PlayOneShot (hoverStart, 1.0f);
	}

	public void PlayHoverCompleteSFX() {
		menuSpeaker3.Stop();
		menuSpeaker3.PlayOneShot(hoverComplete,1.0f);
	}

	void Start() {
		handSelection.updateMode = AnimatorUpdateMode.UnscaledTime;
		handCalibration.updateMode = AnimatorUpdateMode.UnscaledTime;
		handComplete.updateMode = AnimatorUpdateMode.UnscaledTime;
		guideHand.updateMode = AnimatorUpdateMode.UnscaledTime;

		Begin ();
	}

	public void Begin() {
		StartCoroutine("ActivateHandSelectionWindow");
	}

	IEnumerator ActivateHandSelectionWindow() {
		handSelection.gameObject.SetActive(true);
		handSelection.SetTrigger("Activate");

		menuSpeaker1.Play();
		menuSpeaker2.Stop();
		menuSpeaker3.Stop();

		yield return null;
	}

	public void ConfigureLeftHandButton() {
		calibrationPhase = 0;

		calibrationMode = CalibrationMode.Left;

		StartCoroutine("NextPhase");
	}

	public void ConfigureRightHandButton() {
		calibrationPhase = 0;

		calibrationMode = CalibrationMode.Right;

		StartCoroutine("NextPhase");
	}

	IEnumerator NextPhase() {
		switch (++calibrationPhase) {
			case 1:
				StartCoroutine("DeactivateHandSelectionWindow");

				yield return new WaitForSecondsRealtime(0.5f);

				StartCoroutine("ActivateRecordingWindow");

				guideHand.SetTrigger("NeutralFrame");

				calibrationTitle.text = "Copy this wrist movement";

				yield return new WaitForSecondsRealtime(0.5f);

				menuSpeaker1.Play();
				menuSpeaker2.Play();
				menuSpeaker3.Stop();

				guideHand.SetTrigger("1-Wrist");

				StartRecording();
				break;
			case 2:
				StopRecording();
				guideHand.SetTrigger("NeutralFrame");

				StartCoroutine("DeactivateRecordingWindow");

				yield return new WaitForSecondsRealtime(0.5f);

				StartCoroutine("ActivateRecordingWindow");

				calibrationTitle.text = "Copy the fingers and thumb";

				yield return new WaitForSecondsRealtime(0.5f);

				menuSpeaker3.PlayOneShot(complete1,1.0f);

				guideHand.SetTrigger("2-Proximals");

				StartRecording();
				break;
			case 3:
				StopRecording();

				guideHand.SetTrigger("NeutralFrame");


				StartCoroutine("DeactivateRecordingWindow");

				yield return new WaitForSecondsRealtime(0.5f);

				StartCoroutine("ActivateRecordingWindow");

				calibrationTitle.text = "Copy thumb rotation";

				yield return new WaitForSecondsRealtime(0.5f);

				menuSpeaker3.PlayOneShot(complete2,1.0f);

				guideHand.SetTrigger("3-Thumbs");

				StartRecording();
				break;
			case 4:
				StopRecording();

				StartCoroutine("DeactivateRecordingWindow");

				yield return new WaitForSecondsRealtime(0.5f);

				StartCoroutine("ActivateCompleteWindow");
				break;
			default:
				break;
		}
		yield return null;
	}

	IEnumerator ActivateRecordingWindow() {
		handCalibration.gameObject.SetActive (true);
		handCalibration.SetTrigger ("Activate");

		yield return null;
	}

	IEnumerator ActivateCompleteWindow() {
		handComplete.gameObject.SetActive (true);
		handComplete.SetTrigger("Activate");

		yield return null;
	}

	IEnumerator DeactivateHandSelectionWindow() {
		handSelection.SetTrigger("Deactivate");

		yield return new WaitForSecondsRealtime(0.5f);

		handSelection.gameObject.SetActive (false);
	}

	IEnumerator DeactivateRecordingWindow() {
		handCalibration.SetTrigger("Deactivate");

		yield return new WaitForSecondsRealtime(0.5f);

		handCalibration.gameObject.SetActive (false);
	}

	IEnumerator DeactivateCompleteWindow() {
		handComplete.SetTrigger("Deactivate");

		yield return new WaitForSecondsRealtime(0.5f);

		handComplete.gameObject.SetActive (false);
	}

	public void RecordingBackButton() {
		StartCoroutine("LastPhase");
	}

	IEnumerator LastPhase() {
		switch (--calibrationPhase) {
			case 0:
				StartCoroutine("DeactivateRecordingWindow");

				guideHand.SetTrigger("Neutral");

				yield return new WaitForSecondsRealtime(0.5f);

				StartCoroutine("ActivateHandSelectionWindow");
				break;
			default:
				StartCoroutine("DeactivateRecordingWindow");

				guideHand.SetTrigger("Neutral");

				yield return new WaitForSecondsRealtime(0.5f);

				calibrationPhase--; // We do this a second time so it can increment to the expected chapter
				StartCoroutine("NextPhase");
				break;
		}
		yield return null;
	}

	public void RecordingNextButton() {
		StartCoroutine("NextPhase");
	}

	public void RestartWithLeftHand() {
		IEnumerator restartLeftHand = RestartCoroutine(true);
		StartCoroutine(restartLeftHand);
	}

	public void RestartWithRightHand() {
		IEnumerator restartRightHand = RestartCoroutine(false);
		StartCoroutine(restartRightHand);
	}

	IEnumerator RestartCoroutine(bool leftHand) {
		StartCoroutine("DeactivateCompleteWindow");

		yield return new WaitForSecondsRealtime(0.5f);

		if (leftHand) {
			ConfigureLeftHandButton();
		} else {
			ConfigureRightHandButton();
		}

		yield return null;
	}

	void StartRecording() {
		switch (calibrationMode) {
		case CalibrationMode.Left:
			switch (calibrationPhase) {
			case 1:
				leftGlove.CalibrationBehaviour.BeginWristCalibration();
				break;
			case 2:
				leftGlove.CalibrationBehaviour.BeginProximalCalibration();
				break;
			case 3:
				leftGlove.CalibrationBehaviour.BeginThumbCalibration();
				break;
			}
			break;
		case CalibrationMode.Right:
			switch (calibrationPhase) {
			case 1:
				rightGlove.CalibrationBehaviour.BeginWristCalibration();
				break;
			case 2:
				rightGlove.CalibrationBehaviour.BeginProximalCalibration();
				break;
			case 3:
				rightGlove.CalibrationBehaviour.BeginThumbCalibration();
				break;
			}
			break;
		}
	}

	void StopRecording() {
		switch (calibrationMode) {
		case CalibrationMode.Left:
			switch (calibrationPhase) {
			case 1:
				leftGlove.CalibrationBehaviour.EndWristCalibration();
				break;
			case 2:
				leftGlove.CalibrationBehaviour.EndProximalCalibration();
				break;
			case 3:
				leftGlove.CalibrationBehaviour.EndThumbCalibration();
				break;
			}
			break;
		case CalibrationMode.Right:
			switch (calibrationPhase) {
			case 1:
				rightGlove.CalibrationBehaviour.EndWristCalibration();
				break;
			case 2:
				rightGlove.CalibrationBehaviour.EndProximalCalibration();
				break;
			case 3:
				rightGlove.CalibrationBehaviour.EndThumbCalibration();
				break;
			}
			break;
		}
	}

	public void CalibrationFinished() {
		if (destroyRigWhenDone) {
			Destroy(transform.root.gameObject);
		} else {
			StartCoroutine("ActivateHandSelectionWindow");

			StartCoroutine("DeactivateCompleteWindow");
			calibrationPhase = 0;

			GetComponentInParent<MaestroSettingsRig>().CalibrationCompleted();
		}
	}
}