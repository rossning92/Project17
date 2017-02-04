using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;

public class VideoTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		// Destroy(gameObject);
		// MediaPlayer mediaPlayer = (MediaPlayer)FindObjectOfType(typeof(MediaPlayer));
		// mediaPlayer.Stop ();

		Camera mainCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera>();
		Camera vidCamera = GameObject.FindGameObjectWithTag ("360VideoCamera").GetComponent<Camera>();
	
		mainCamera.enabled = false;
		vidCamera.enabled = true;
	}

	void OnTriggerExit(Collider other) {

		Camera mainCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera>();
		Camera vidCamera = GameObject.FindGameObjectWithTag ("360VideoCamera").GetComponent<Camera>();

		mainCamera.enabled = true;
		vidCamera.enabled = false;
	}
}
