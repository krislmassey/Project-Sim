using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureExample : MonoBehaviour {

	public bool raycasting;
	/*
	public Transform currentTarget;
	*/

	public Light targetLight;
	public Material targetMaterial;

	RaycastHit hit;
	Collider hitTarget;

	public float deltaZRotation;
	float oldZRotation;
	/*
	float newSize = 1.0f;
	*/

	public void CheckBeginRaycasting(GestureObject gesture) {
		if (string.Equals(gesture.gestureName,"Point")) {
			raycasting = true;
		}
	}

	public void CheckEndRaycasting(GestureObject gesture) {
		if (string.Equals(gesture.gestureName,"Point")) {
			raycasting = false;
		}
	}

	void Update() {
		if (raycasting) {
			RaycastHit[] temp = Physics.SphereCastAll(transform.position, 0.1f, transform.forward, 100.0f);
            Debug.DrawLine(transform.position, transform.position + (100.0f * transform.forward.normalized), Color.cyan);

            foreach (RaycastHit hit in temp)
            {
                if (hit.collider)
                { // If we hit something
                    if (hitTarget)
                    { // And we already have a target
                        if (hitTarget != hit.collider)
                        { // and the thing we hit isn't out current target
                            AssignNewHitTarget(hit.collider); // Store it and check if its something we can scale
                            if (targetLight)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        AssignNewHitTarget(hit.collider); // If we don't have a target (first ray), store it and check if its something we can scale
                        if (targetLight)
                        {
                            break;
                        }
                    }
                }
            }

			/*
			if (currentTarget) {
				deltaZRotation = (transform.eulerAngles.z - oldZRotation) * 0.25f;
				oldZRotation = transform.eulerAngles.z;

				newSize = Mathf.Lerp(newSize,Mathf.Clamp((currentTarget.parent.localScale.x - deltaZRotation), 1.0f, 2.0f), Time.deltaTime * 4.0f);

				currentTarget.parent.localScale = new Vector3(newSize,newSize,newSize);
			}
			*/

			deltaZRotation = (transform.eulerAngles.z - oldZRotation) * 2.0f;
			oldZRotation = transform.eulerAngles.z;

			if (targetLight) {
				targetLight.intensity = Mathf.Lerp(targetLight.intensity,Mathf.Clamp(targetLight.intensity-deltaZRotation,0f,1.0f),Time.deltaTime);
			}

			if (targetMaterial) {
				targetMaterial.SetColor("_EmissionColor",new Color(targetLight.intensity * 1.0f,targetLight.intensity * 1.0f,targetLight.intensity * 1.0f));
			}
		}
	}

	void AssignNewHitTarget(Collider c) {
		hitTarget = c;
        
		/*
		if (hitTarget.GetComponent<GestureExampleTarget>()) {
			currentTarget = hitTarget.transform.GetChild(0).GetChild(0);
		} else {
			currentTarget = null;
		}
		*/

		if (hitTarget.GetComponent<GestureExampleTarget>()) {
			//targetLight = hitTarget.GetComponentInChildren<Light>();
			//targetMaterial = hitTarget.GetComponentInChildren<MeshRenderer>().material;

			targetLight = hitTarget.GetComponent<Light>();
			targetMaterial = hitTarget.GetComponent<MeshRenderer>().material;

			targetMaterial.SetColor("_EmissionColor",new Color(targetLight.intensity * 2.0f,targetLight.intensity * 2.0f,targetLight.intensity * 2.0f));
		} else {
			targetLight = null;
			targetMaterial = null;
			//currentTarget = null;
		}
	}
}