//=============================================================================
//
// Purpose: Interprets knuckles input and animates a hand model
//
//=============================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class MaestroHandPose{

    [DllImport("MaestroAPI.dll")]
    private static extern float get_index_proximal_rotation_ratio(IntPtr glove);

    [DllImport("MaestroAPI.dll")]
    private static extern float get_middle_proximal_rotation_ratio(IntPtr glove);

    [DllImport("MaestroAPI.dll")]
    private static extern float get_ring_proximal_rotation_ratio(IntPtr glove);

    [DllImport("MaestroAPI.dll")]
    private static extern float get_little_proximal_rotation_ratio(IntPtr glove);

    [DllImport("MaestroAPI.dll")]
    private static extern float get_thumb_proximal_rotation_ratio(IntPtr glove);

    [DllImport("MaestroAPI.dll")]
    private static extern bool is_glove_connected(IntPtr glove);

    public float index_curl(IntPtr glove) { return is_glove_connected(glove) ? get_index_proximal_rotation_ratio(glove) : -1f; } 
    public float middle_curl(IntPtr glove) { return is_glove_connected(glove) ? get_middle_proximal_rotation_ratio(glove) : -1f; }
    public float ring_curl(IntPtr glove) { return is_glove_connected(glove) ? get_ring_proximal_rotation_ratio(glove) : -1f; }
    public float pinky_curl(IntPtr glove) { return is_glove_connected(glove) ? get_little_proximal_rotation_ratio(glove) : -1f; }
    public float thumb_lift(IntPtr glove) { return is_glove_connected(glove) ? get_thumb_proximal_rotation_ratio(glove) : -1f; }

    public Vector2 thumbPos = Vector2.zero;
	public float squeeze = 0.0f;

	public MaestroHandPose(){

	}
}

public class MaestroHandControl : MonoBehaviour {

    [DllImport("MaestroAPI.dll")]
    private static extern IntPtr get_left_glove_pointer();

    [DllImport("MaestroAPI.dll")]
    private static extern IntPtr get_right_glove_pointer();


    public enum WhichHand{
		Left,
		Right
	}

	public WhichHand whichHand;

    public SteamVR_TrackedObject controller;

   // private SocketService sockserv;
	[SerializeField]
    //private Animator anim;

	public MaestroHandPose handPose;

    public float IndexCurl;
    public float MiddleCurl;
    public float RingCurl;
    public float PinkyCurl;
    public float ThumbLift;

    public bool HandOpen;
    public bool HandClosed;

	[HideInInspector]
	public SteamVR_Controller.Device vrcontroller;

	[Tooltip("use older vr controllers for testing. Trigger controls all fingers")]
	public bool emulate;

    private IntPtr glove;

	float[] clamps = new float[6];

	public bool handOpenFull{
		get{
			if (handPose != null) {
				return handPose.thumb_lift(glove) > 0.6f && handPose.index_curl(glove) > 0.6f && handPose.middle_curl(glove) > 0.6f && handPose.ring_curl(glove) > 0.6f && handPose.pinky_curl(glove) > 0.6f;
			} else {
				return false;
			}
		}
	}

	public bool handClosedFull{
		get{
            if (handPose != null)
            {
                return handPose.thumb_lift(glove) < 0.1f && handPose.index_curl(glove) < 0.1f && handPose.middle_curl(glove) < 0.1f && handPose.ring_curl(glove) < 0.1f && handPose.pinky_curl(glove) < 0.1f;

            } else
            {
                return false;
            }
        }
	}

    void Start() {
		handPose = new MaestroHandPose ();

		vrcontroller = SteamVR_Controller.Input((int)controller.index);
    }

	public void SetClamp(int finger, float val = -1){
		if (val == -1) {
			val = getposfromindex (finger);

			clamps [finger] = val;
		} else {
			clamps[finger] = val;
		}
	}

	public float getposfromindex(int i){
		float val = 1;
		if (i == 0)
			val = handPose.thumb_lift(glove);
		if (i == 1)
			val = handPose.index_curl(glove);
		if (i == 2)
			val = handPose.middle_curl(glove);
		if (i == 3)
			val = handPose.ring_curl(glove);
		if (i == 4)
			val = handPose.pinky_curl(glove);
		//if (i > 4 || i < 0)
		//	val = 1;

		return val;
	}

	public void TriggerHaptics(int length){
		if (vrcontroller != null) {
			vrcontroller.TriggerHapticPulse ((ushort)length);
		}
	}
    
    void Update() {
		vrcontroller = SteamVR_Controller.Input((int)controller.index);

        if (whichHand == WhichHand.Left)
            glove = get_left_glove_pointer();
        else
            glove = get_right_glove_pointer();

        //Debug variables
        IndexCurl = handPose.index_curl(glove);
        MiddleCurl = handPose.middle_curl(glove);
        RingCurl = handPose.ring_curl(glove);
        PinkyCurl = handPose.pinky_curl(glove);
        ThumbLift = handPose.thumb_lift(glove);
        HandOpen = handOpenFull;
        HandClosed = handClosedFull;
    }
}
