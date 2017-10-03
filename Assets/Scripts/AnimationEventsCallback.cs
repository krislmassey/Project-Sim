using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventsCallback : MonoBehaviour {
	
	public UnityEvent OnAnimationEvent;

	void AnimationEvent() {

		OnAnimationEvent.Invoke();


	}
}
