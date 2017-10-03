using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {

  public Vector3 rotation;
  public float rotationSpeed;
	// Use this for initialization

	void Start () {

	}

	// Update is called once per frame
	void Update () {

    transform.Rotate (rotation * rotationSpeed * Time.unscaledDeltaTime);

	}
}
