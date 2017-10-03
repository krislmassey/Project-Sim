using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Maestro.Phalanges;
using Maestro.Phalanges.Distal;
using System.Linq;

public class VRButton : MonoBehaviour {

	public UnityEvent OnTouch;
	public UnityEvent OnPush;
	public UnityEvent OnRelease;

	public float pushDepth = 0.05f;
	public bool pushed;

	Vector3 startPos;

    private List<DistalPhalange> collidedDistalPhalanges;

	void Start() {
		startPos = transform.localPosition;
        collidedDistalPhalanges = new List<DistalPhalange>();
    }

	void OnCollisionEnter() {
		OnTouch.Invoke();
	}

    void OnTriggerEnter(Collider other)
    {
        var distalPhalanges = other.gameObject.GetComponentInParent<PhalangeBehaviour>().Phalanges.Where(ph => ph is DistalPhalange).Cast<DistalPhalange>().ToList();

        if (distalPhalanges.Count > 0)
        {
            collidedDistalPhalanges.AddRange(distalPhalanges);
        }
    }

    void Update() {
		if (!pushed) {
			if (transform.localPosition.z > (startPos.z + pushDepth)) {
				pushed = true;
				OnPush.Invoke();
			}
		} else {
			if (transform.localPosition.z < (startPos.z + 0.01f)) {
				pushed = false;

                collidedDistalPhalanges.ForEach(dp => {
                    dp.ResetHapticsEffect();
                });

                collidedDistalPhalanges.Clear();

                OnRelease.Invoke();
            }
		}
	}

	public void TestTouch() {
		DebugLogger.DebugMessage(gameObject,"That's sweet, I'm touched.");
	}

	public void TestPush() {
		DebugLogger.DebugMessage(gameObject,"I've been pushed around way too long!");
	}

	public void TestRelease() {
		DebugLogger.DebugMessage(gameObject,"At last, I am released.");
	}
}
