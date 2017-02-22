﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoTrigger : MonoBehaviour {
	
	public string videoFile;
	public DemoApp.VideoSphereType videoSphereType;
	public Transform videoSphereRotation;
	public float RestartVideoAfterSecs = 0;


	private DemoApp _app;


	void Start() {
		_app = (DemoApp)FindObjectOfType (typeof(DemoApp));
	}

	void OnTriggerEnter(Collider other) {
		_app.StartVideo (this);
	}

	void OnTriggerExit(Collider other) {
		_app.StopVideo ();
	}

	public bool IsTriggerActive() {
		return _app.GetCurrentVideoTrigger () == this;
	}
}
