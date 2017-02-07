using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;

public class VideoTrigger : MonoBehaviour {
	private float fadeTextureAlpha = 0.0f;
	public float fadeSpeed = 2.0f;
	public string videoFile;

	private enum Status
	{
		InMainCamera,
		MainCameraFadeOut,
		VideoCameraFadeIn,
		InVideoCamera,
		VideoCameraFadeOut,
		MainCameraFadeIn
	};
	private Status status = Status.InMainCamera;
	private bool areaEntered = false;
	private bool areaExited = false;


	private MediaPlayer mediaPlayer;
	private DemoApp _app;


	void Fade(GameObject obj, bool fadeOut) {
		if (fadeOut) {
			fadeTextureAlpha += Time.deltaTime * fadeSpeed;
		} else {
			fadeTextureAlpha -= Time.deltaTime * fadeSpeed;
		}

		FadeEffect fadeEffect = obj.GetComponentInChildren<FadeEffect> ();
		fadeEffect.SetAlpha (fadeTextureAlpha);
	}

	// Use this for initialization
	void Start () {
		mediaPlayer = (MediaPlayer)FindObjectOfType (typeof(MediaPlayer));
		_app = FindObjectOfType<DemoApp> ();
	}

	void SwitchToVideoCamera ()
	{
		DemoApp app = FindObjectOfType<DemoApp> ();
		//app.mainCamera.SetActive (false);
		app.videoCamera.SetActive (true);

		mediaPlayer.OpenVideoFromFile (
			MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, 
			videoFile);
		mediaPlayer.Play ();
	}

	void SwitchToMainCamera ()
	{
		DemoApp app = FindObjectOfType<DemoApp> ();
		//app.mainCamera.SetActive (true);
		app.videoCamera.SetActive (false);
	}

	// Update is called once per frame
	void Update () {
		if (status == Status.InMainCamera) {
			
			if (areaEntered) {
				areaEntered = false;
				status = Status.MainCameraFadeOut;
			}

		} else if (status == Status.MainCameraFadeOut) {
			
			if (fadeTextureAlpha >= 1.0f) {
				SwitchToVideoCamera ();
				status = Status.VideoCameraFadeIn;
			} else {
				Fade (_app.mainCamera, true);
			}

		} else if (status == Status.VideoCameraFadeIn) {
			
			if (fadeTextureAlpha <= 0.0f) {
				status = Status.InVideoCamera;
			} else {
				Fade (_app.videoCamera, false);
			}

		} else if (status == Status.InVideoCamera) {

			if (areaExited) {
				areaExited = false;
				status = Status.VideoCameraFadeOut;
			}

		} else if (status == Status.VideoCameraFadeOut) {

			if (fadeTextureAlpha >= 1.0f) {
				SwitchToMainCamera ();
				status = Status.MainCameraFadeIn;
			} else {
				Fade (_app.videoCamera, true);
			}

		} else if (status == Status.MainCameraFadeIn) {

			if (fadeTextureAlpha <= 0.0f) {
				status = Status.InMainCamera;
			} else {
				Fade (_app.mainCamera, false);
			}

		}

	}

	void OnTriggerEnter(Collider other) {

		// Destroy(gameObject);
		// MediaPlayer mediaPlayer = (MediaPlayer)FindObjectOfType(typeof(MediaPlayer));
		// mediaPlayer.Stop ();

		areaEntered = true;

	}

	void OnTriggerExit(Collider other) {

		areaExited = true;

	}
}
