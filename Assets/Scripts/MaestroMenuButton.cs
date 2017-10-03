using UnityEngine;
using UnityEngine.Events;

public class MaestroMenuButton : MonoBehaviour {

	public UnityEvent OnButtonTouch;
	public UnityEvent OnButtonPush;
	public UnityEvent OnButtonRelease;

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