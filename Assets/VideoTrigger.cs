using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;

public class VideoTrigger : MonoBehaviour {
	
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
	private Status _status = Status.InMainCamera;
	private bool _areaEntered = false;
	private bool _areaExited = false;


	private MediaPlayer _mediaPlayer;
	private DemoApp _app;
	private float _fadeTextureAlpha = 0.0f;

	void Fade(GameObject obj, bool fadeOut) {
		if (fadeOut) {
			_fadeTextureAlpha += Time.deltaTime * fadeSpeed;
		} else {
			_fadeTextureAlpha -= Time.deltaTime * fadeSpeed;
		}

		FadeEffect fadeEffect = obj.GetComponentInChildren<FadeEffect> ();
		fadeEffect.SetAlpha (_fadeTextureAlpha);
	}

	// Use this for initialization
	void Start () {
		_mediaPlayer = (MediaPlayer)FindObjectOfType (typeof(MediaPlayer));
		_app = FindObjectOfType<DemoApp> ();
	}

	void SwitchToVideoCamera ()
	{
		DemoApp app = FindObjectOfType<DemoApp> ();
		//app.mainCamera.SetActive (false);
		app.videoCamera.SetActive (true);

		_mediaPlayer.OpenVideoFromFile (
			MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, 
			videoFile);
		_mediaPlayer.Play ();
	}

	void SwitchToMainCamera ()
	{
		DemoApp app = FindObjectOfType<DemoApp> ();
		//app.mainCamera.SetActive (true);
		app.videoCamera.SetActive (false);
	}

	// Update is called once per frame
	void Update () {
		if (_status == Status.InMainCamera) {
			
			if (_areaEntered) {
				_areaEntered = false;
				_status = Status.MainCameraFadeOut;
			}

		} else if (_status == Status.MainCameraFadeOut) {
			
			if (_fadeTextureAlpha >= 1.0f) {
				SwitchToVideoCamera ();
				_status = Status.VideoCameraFadeIn;
			} else {
				Fade (_app.mainCamera, true);
			}

		} else if (_status == Status.VideoCameraFadeIn) {
			
			if (_fadeTextureAlpha <= 0.0f) {
				_status = Status.InVideoCamera;
			} else {
				Fade (_app.videoCamera, false);
			}

		} else if (_status == Status.InVideoCamera) {

			if (_areaExited) {
				_areaExited = false;
				_status = Status.VideoCameraFadeOut;
			}

		} else if (_status == Status.VideoCameraFadeOut) {

			if (_fadeTextureAlpha >= 1.0f) {
				SwitchToMainCamera ();
				_status = Status.MainCameraFadeIn;
			} else {
				Fade (_app.videoCamera, true);
			}

		} else if (_status == Status.MainCameraFadeIn) {

			if (_fadeTextureAlpha <= 0.0f) {
				_status = Status.InMainCamera;
			} else {
				Fade (_app.mainCamera, false);
			}

		}

	}

	void OnTriggerEnter(Collider other) {
		_areaEntered = true;

		Debug.Log ("Enter");
	}

	void OnTriggerExit(Collider other) {
		_areaExited = true;

		Debug.Log ("Exit");
	}
}
