using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ViveInteractionManager : MonoBehaviour {

	[SerializeField] GameObject targetObj;
	[SerializeField] GameObject cameraRig;
	[SerializeField] GameObject mainCam;
	[SerializeField] GameObject controller;
	[SerializeField] float radius = 0.5f;

	Vector3 basisVec;
	Vector3 offsetPosition;
	Quaternion offsetRotation;
	Quaternion defaultHeadRotationInverse;
	float multiplier = 1.0f;

	SteamVR_Controller.Device _controller;

	void Awake()
	{
		InputTracking.trackingAcquired += InputTracking_trackingAcquired;
	}



	void InputTracking_trackingAcquired (XRNodeState obj)
	{
		InputTracking.trackingAcquired -= InputTracking_trackingAcquired;

		defaultHeadRotationInverse = Quaternion.Inverse (mainCam.transform.rotation);

		basisVec = mainCam.transform.forward;
		basisVec.y = 0.0f;
		basisVec = basisVec.normalized * radius;	

	}


	void Update ()
	{
		_controller = SteamVR_Controller.Input ((int)controller.GetComponent<SteamVR_TrackedObject> ().index);

		if (_controller.GetPress (SteamVR_Controller.ButtonMask.Touchpad)) {
			
			multiplier = Mathf.Max (multiplier - _controller.GetAxis ().y * 0.03f, 0.1f);
			//print ("pressed");
		}

		//calculate offset between mainCamer and cameraRig
		offsetPosition = mainCam.transform.position - cameraRig.transform.position;

		//calculate HMD rotation
		offsetRotation = mainCam.transform.rotation * defaultHeadRotationInverse;

		//update cameraRig position
		cameraRig.transform.position = targetObj.transform.position - offsetRotation * basisVec * multiplier - offsetPosition;
	}


}
