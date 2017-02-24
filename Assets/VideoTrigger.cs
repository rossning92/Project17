using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoTrigger : MonoBehaviour {
	
	public string videoFile;
	public DemoApp.VideoSphereType videoSphereType;
	public Transform videoSphereRotation;
	public float RestartVideoAfterSecs = 0;
	public float radiusToEnter = 0.5f;
	public float radiusToExit = 1.0f;

	private DemoApp _app;
	private bool _triggered = false;

	void Start() {
		_app = (DemoApp)FindObjectOfType (typeof(DemoApp));
	}

	void Update() {

		Vector3 triggerPos = transform.position;
		Vector2 triggerPos2D = new Vector2 (triggerPos.x, triggerPos.z);

		Vector3 cameraPos = DemoApp.Instance.mainCamera.transform.position;
		Vector2 cameraPos2D = new Vector2 (cameraPos.x, cameraPos.z);

		float d = (cameraPos2D - triggerPos2D).magnitude;
		if (!_triggered && d < radiusToEnter) {

			_app.StartVideo (this);
			_triggered = !_triggered;
		
		} else if (_triggered && d > radiusToExit) {

			_app.StopVideo ();
			_triggered = !_triggered;
		
		}

	}


	/*

	void OnTriggerEnter(Collider other) {
		_app.StartVideo (this);
	}

	void OnTriggerExit(Collider other) {
		_app.StopVideo ();
	}

	*/


	public bool IsTriggerActive() {
		return _app.GetCurrentVideoTrigger () == this;
	}
}
