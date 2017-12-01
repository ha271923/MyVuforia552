using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class vBtnScript : MonoBehaviour, IVirtualButtonEventHandler {
	public GameObject virtualButtonObject;
	
	// Use this for initialization
	void Start () {
		Debug.Log("vBtnScript:: Start");
		virtualButtonObject = GameObject.Find("MyVirtualButton1");
		virtualButtonObject.GetComponent<VirtualButtonBehaviour>().RegisterEventHandler(this);

	}
	
	// Update is called once per frame
	void Update () {
		// Debug.Log("Update");
	}

	public void OnButtonPressed(VirtualButtonAbstractBehaviour vb)
	{
		Debug.Log("OnButtonPressed +++");
	}

	public void OnButtonReleased(VirtualButtonAbstractBehaviour vb)
	{
		Debug.Log("OnButtonReleased ---");
	}


}
