using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoTrigger : MonoBehaviour {
	
	public string videoFile;
	public DemoApp.VideoSphereType videoSphereType;
	public Transform videoSphereRotation;

	private DemoApp _app;

	void Start() {
		_app = (DemoApp)FindObjectOfType (typeof(DemoApp));
	}

	void OnTriggerEnter(Collider other) {
		_app.StartVideo (videoFile, videoSphereType, videoSphereRotation);
	}

	void OnTriggerExit(Collider other) {
		_app.StopVideo ();
	}
}
