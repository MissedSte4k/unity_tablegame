using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CameraControl : NetworkBehaviour {

	private float mouseH = 0.0f;
	private float mouseV = 0.0f;
	public float mouseSensitivity;
    public Camera playerCamera;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (isLocalPlayer)
        {
            mouseH += Input.GetAxis("Mouse X") * mouseSensitivity;
            mouseV -= Input.GetAxis("Mouse Y") * mouseSensitivity;

            playerCamera.transform.rotation = Quaternion.Euler(Mathf.Clamp(mouseV, -90, 90), mouseH, 0);
        }
        else playerCamera.enabled = false;
	}
}
