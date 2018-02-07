using UnityEngine;
using UnityEngine.Events;
using System.Collections;
//using UnityEngine.EventSystems;

public class GazeRay : MonoBehaviour {

	public static GazeRay instance;

	RaycastHit hit;
	int layerMask;

	public GazeTarget gazeTarget;

	//[HideInInspector]
	public bool TargetHit;

	
	public bool RayHit;


	[HideInInspector]
	public bool Deactivated;

	[HideInInspector]
	public bool circleVisible;

	[HideInInspector]
	public float timer;

	[HideInInspector]
	public float timerLength;

	[HideInInspector]
	public float currentAngle;

	[HideInInspector]
	public float angleBetweenPoints;

	[HideInInspector]
	public int vertexCount = 90;

	[HideInInspector]
	public float radius = 0.05f;

	[HideInInspector]
	public float tickNormal;

	public bool UseUnscaledTime;

	[HideInInspector]
	public float lineRendererLocalZDepth = 1.0f;
	LineRenderer lineRenderer;
	Material lineRendMaterial;

	public Sprite reticleSprite;

	public UnityEvent OnGazeBegin;
	public UnityEvent OnGazeComplete;
	public UnityEvent OnGazeCancel;

	void Awake() {
		instance = this;

		InitializeChildObjects ();

		layerMask = 1 << LayerMask.NameToLayer("Calibration");
	}
	
	void InitializeChildObjects() {
		// Initialize child object
		GameObject lineRendererChild = new GameObject ();
		lineRendererChild.transform.SetParent (transform);
		lineRendererChild.transform.localPosition = new Vector3 (0, 0, lineRendererLocalZDepth);
		lineRendererChild.transform.localEulerAngles = new Vector3 (0, 0, 90);
		lineRendererChild.name = "GazeCircle";
		lineRendererChild.layer = LayerMask.NameToLayer("Calibration");

		// Initialize LineRenderer component
		lineRenderer = lineRendererChild.AddComponent<LineRenderer> ();
		lineRenderer.receiveShadows = false;
		lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		lineRenderer.useWorldSpace = false;
		lineRenderer.widthMultiplier = radius * 0.5f;
		lineRenderer.endWidth = radius * 0.5f;
		lineRenderer.startWidth = radius * 0.5f;

		// Initialize LineRenderer material
		lineRendMaterial = new Material (Shader.Find ("Particles/Alpha Blended"));
		lineRendMaterial.name = "GazeCircle";
		lineRenderer.material = lineRendMaterial;

		lineRenderer.enabled = false;

		// Initialize reticle object
		GameObject reticleChild = new GameObject ();
		reticleChild.name = "GazeReticle";
		reticleChild.transform.SetParent (lineRendererChild.transform);
		reticleChild.transform.localScale *= 0.1f;
		reticleChild.transform.localPosition = Vector3.zero;
		reticleChild.transform.localEulerAngles = Vector3.zero;
		reticleChild.layer = LayerMask.NameToLayer("Calibration");
		SpriteRenderer reticleSpriteRenderer = reticleChild.AddComponent<SpriteRenderer> ();
		reticleSpriteRenderer.sprite = reticleSprite;
		reticleSpriteRenderer.color = new Color (1, 1, 1, 0.5f);
	}

	void Update () {
		if (!Deactivated) {
			DoGazeRay ();
		}
	}

	void DoGazeRay() {
		TargetHit = false;
		RayHit = true;

		Debug.DrawRay(transform.position,transform.forward * 50,Color.blue);
		Physics.Raycast(transform.position,transform.forward,out hit,50.0f,layerMask);

		if (hit.collider) { // If our ray connects...
			RayHit = true;
			if (gazeTarget) { // And we already have a target assigned...
				if (gazeTarget.gameObject == hit.collider.gameObject) { // And they're the same object...
					GazeFrame(); // Proceed with the gaze timer.
				} else { // But if they're not...
					UnassignTarget(); // Unassign the old target...
					AssignTarget(hit.transform.GetComponent<GazeTarget>()); // And assign the new one.
				}
			} else if (hit.collider.GetComponent<GazeTarget>()) { // If our ray connects, we don't have a target assigned, but the object we hit is a target...
				AssignTarget(hit.transform.GetComponent<GazeTarget>()); // Assign the new target.
			}
		} else {
			RayHit = false;

			if (gazeTarget) {
				UnassignTarget();
			}
		}
	}

	void AssignTarget(GazeTarget target) {
		TargetHit = true;
		gazeTarget = target;
		InitializeTimer();
		OnGazeBegin.Invoke();
		gazeTarget.OnTargetBegin.Invoke();
	}

	void UnassignTarget() {
		gazeTarget.OnTargetCancel.Invoke();
		OnGazeCancel.Invoke();

		if (circleVisible)
			ResetLinePoints();

		Reset();
		gazeTarget = null;
	}

	public void GazeComplete() {
		StartCoroutine("TemporaryDeactivation");

		OnGazeComplete.Invoke(); 
		gazeTarget.GazeComplete ();

		if (circleVisible) {
			StartCoroutine ("FadeOutLineRenderer");
			Reset();
		} else {
			Reset ();
		}
	}

	IEnumerator TemporaryDeactivation() {
		Deactivated = true;

		yield return new WaitForSecondsRealtime(1.0f);

		Deactivated = false;
	}

	public void InitializeTimer() {
		timerLength = gazeTarget.timerLength;
		timer = 0;
		tickNormal = 0;
		circleVisible = gazeTarget.circleVisible;

		if (circleVisible) {
			currentAngle = 0;
			angleBetweenPoints = 360 / vertexCount;

			lineRenderer.positionCount = (vertexCount + 1);

			ResetLinePoints ();

			lineRenderer.enabled = true;

			ChangeLineColor (gazeTarget.circleColor);
		}
	}

	public void GazeFrame() {
		if (timer < timerLength) {

			if (UseUnscaledTime) {
				timer += Time.unscaledDeltaTime;
			} else {
				timer += Time.deltaTime;
			}

			tickNormal = (float)(timer / timerLength);
			currentAngle = Mathf.Lerp(0,360,tickNormal);
		} else {
			GazeComplete ();
			return;
		}

		if (circleVisible) {
			for (int i = 0; i < vertexCount+1; i++) {
				if (i < tickNormal * vertexCount) {
					lineRenderer.SetPosition(i,GetPosition(i * angleBetweenPoints));
				} else {
					lineRenderer.SetPosition (i,GetPosition (currentAngle));
				}
			}
		}
	}

	Vector3 GetPosition(float angle) {
		float pX = radius * Mathf.Cos (angle * Mathf.PI / 180);
		float pY = -(radius * Mathf.Sin (angle * Mathf.PI / 180));

		return new Vector3(pX,pY,0);
	}

	void Reset() {
		timer = 0;
		tickNormal = 0;
		currentAngle = 0;
		
		gazeTarget = null;
		TargetHit = false;
	}

	IEnumerator FadeOutLineRenderer() {
		Color fadeColor = lineRendMaterial.GetColor("_TintColor");
		float a = fadeColor.a;

		while (a > 0) {
			a -= 0.025f;
			fadeColor.a = a;
			lineRendMaterial.SetColor("_TintColor",fadeColor);
			yield return null;
		}

		fadeColor.a = 0;
		lineRendMaterial.SetColor("_TintColor",fadeColor);

		ResetLinePoints ();
		lineRenderer.enabled = false;
		yield return null;
	}

	void ChangeLineColor(Color color) {
		lineRendMaterial.SetColor("_TintColor",color);
	}

	void ResetLinePoints() {
		for (int i = 0; i < vertexCount+1; i++) {
			lineRenderer.SetPosition(i,Vector3.zero);
		}
	}
}