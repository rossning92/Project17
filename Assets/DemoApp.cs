using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;

public class DemoApp : MonoBehaviour {

	public Camera mainCamera;
	public GameObject videoSphere;
	public float fadeSpeed = 2.0f;
	private int oldCullingMask;


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
	private float _fadeTextureAlpha = 0.0f;
	private string _curVideoFile;


	// Use this for initialization
	void Start () {
		_mediaPlayer = (MediaPlayer)FindObjectOfType (typeof(MediaPlayer));

		videoSphere.SetActive (false);
	}

	private void UpdateVideoSpherePos() {
		videoSphere.transform.position = mainCamera.transform.position;
	}

	private void Fade(Camera obj, bool fadeOut) {
		if (fadeOut) {
			_fadeTextureAlpha += Time.deltaTime * fadeSpeed;
		} else {
			_fadeTextureAlpha -= Time.deltaTime * fadeSpeed;
		}

		FadeEffect fadeEffect = obj.GetComponentInChildren<FadeEffect> ();
		fadeEffect.SetAlpha (_fadeTextureAlpha);
	}

	private void SwitchToVideoCamera ()
	{
		DemoApp app = FindObjectOfType<DemoApp> ();


		videoSphere.SetActive (true);



		oldCullingMask = mainCamera.cullingMask;
		mainCamera.cullingMask = 1 << LayerMask.NameToLayer ("VideoSphere")
				| 1 << LayerMask.NameToLayer ("TransparentFX");


		// play video file
		_mediaPlayer.OpenVideoFromFile (
			MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, 
			_curVideoFile);
		_mediaPlayer.Play ();
	}

	private void SwitchToMainCamera ()
	{
		DemoApp app = FindObjectOfType<DemoApp> ();

		mainCamera.cullingMask = oldCullingMask;

		videoSphere.SetActive (false);


		// stop playing video file
		_mediaPlayer.Stop();
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
				Fade (mainCamera, true);
			}

		} else if (_status == Status.VideoCameraFadeIn) {

			UpdateVideoSpherePos ();

			if (_fadeTextureAlpha <= 0.0f) {
				_status = Status.InVideoCamera;
			} else {
				Fade (mainCamera, false);
			}

		} else if (_status == Status.InVideoCamera) {

			UpdateVideoSpherePos ();

			if (_areaExited) {
				_areaExited = false;
				_status = Status.VideoCameraFadeOut;
			}

		} else if (_status == Status.VideoCameraFadeOut) {

			UpdateVideoSpherePos ();

			if (_fadeTextureAlpha >= 1.0f) {
				SwitchToMainCamera ();
				_status = Status.MainCameraFadeIn;
			} else {
				Fade (mainCamera, true);
			}

		} else if (_status == Status.MainCameraFadeIn) {

			if (_fadeTextureAlpha <= 0.0f) {
				_status = Status.InMainCamera;
			} else {
				Fade (mainCamera, false);
			}

		}

	}

	public void StartVideo(string fileName) {
		_areaEntered = true;
		_curVideoFile = fileName;
	}

	public void StopVideo() {
		_areaExited = true;
	}

}
