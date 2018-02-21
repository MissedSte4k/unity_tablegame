using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

	private float mouseH = 0.0f;
	private float mouseV = 0.0f;
	public float mouseSensitivity;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		mouseH += Input.GetAxis ("Mouse X") * mouseSensitivity;
		mouseV -= Input.GetAxis ("Mouse Y") * mouseSensitivity;

		transform.rotation = Quaternion.Euler (Mathf.Clamp(mouseV, -90, 90), mouseH, 0);
	}
}
