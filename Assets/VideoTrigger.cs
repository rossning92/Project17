using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoTrigger : MonoBehaviour {
	
	public string videoFile;

	private DemoApp _app;

	void Start() {
		_app = (DemoApp)FindObjectOfType (typeof(DemoApp));
	}

	void OnTriggerEnter(Collider other) {
		_app.StartVideo (videoFile);
	}

	void OnTriggerExit(Collider other) {
		_app.StopVideo ();
	}
}
