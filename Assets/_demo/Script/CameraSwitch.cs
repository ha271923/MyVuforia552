using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
	public GameObject camera0;
	public GameObject camera1;
	public GameObject camera2;
	public GameObject selectedCamera;

	// Use this for initialization
	void Start()
	{
		Debug.Log("CameraSwitch::Start()");
		//Camera Position Set
		// cameraPositionChange(PlayerPrefs.GetInt("CameraPosition"));
		camera0.SetActive(true);
		camera1.SetActive(true);
		camera2.SetActive(true);
	}

	// Update is called once per frame
	void Update()
	{
		//Change Camera Keyboard
		switchCamera();
	}

	//UI JoyStick Method
	public void cameraPositonM()
	{
		Debug.Log("CameraSwitch::cameraPositonM()");
		// TODO: add another control at here!!
	}

	//Change Camera Keyboard
	void switchCamera()
	{
		// Righ hand Number Pad # only
		if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Keypad2))
		{
			int cameraPosition = 0;
			// int cameraPosition = PlayerPrefs.GetInt("CameraPosition");
			if (Input.GetKeyDown(KeyCode.Keypad0))
			{
				Debug.Log("CameraSwitch::switchCamera() detect key event #0");
				cameraPosition = 0;
			}
			if (Input.GetKeyDown(KeyCode.Keypad1))
			{
				Debug.Log("CameraSwitch::switchCamera() detect key event #1");
				cameraPosition = 1;
			}
			if (Input.GetKeyDown(KeyCode.Keypad2))
			{
				Debug.Log("CameraSwitch::switchCamera() detect key event #2");
				cameraPosition = 2;
			}
			// cameraPositionChange(cameraPosition);
			cameraPositionChange(cameraPosition);
		}
	}

	//Camera change Logic
	void cameraPositionChange(int camPosition)
	{

		Debug.Log("CameraSwitch::cameraPositionChange(" + camPosition+")");
		// Set default if not handle
		if (camPosition > 2)
		{
			camPosition = 0;
		}

		//Set camera position database
		PlayerPrefs.SetInt("CameraPosition", camPosition);

		//Set camera position
		if (camPosition == 0)
			selectedCamera = camera0;
		if (camPosition == 1)
			selectedCamera = camera1;
		if (camPosition == 2)
			selectedCamera = camera2;

		if (selectedCamera.activeInHierarchy == true)
			selectedCamera.SetActive(false);
		else
			selectedCamera.SetActive(true);
	}
}