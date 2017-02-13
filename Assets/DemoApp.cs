using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;

public class DemoApp : MonoBehaviour {

	public enum VideoSphereType {
		NormalVideoSphere,
		TransparentVideoSphere,
		VideoSphereWithViveCamera
	};

	private enum Status
	{
		InMainCamera,
		MainCameraFadeOut,
		VideoCameraFadeIn,
		InVideoCamera,
		VideoCameraFadeOut,
		MainCameraFadeIn
	};

	public Camera mainCamera;
	public GameObject videoSphere;
	public GameObject fadeEffect;
	public float fadeSpeed = 2.0f;
	public MediaPlayer mediaPlayer;


	private int _oldCullingMask;
	private VideoSphereType _curVideoSphereType;
	private Transform _curVideoSphereRotation;
	private Status _status = Status.InMainCamera;
	private bool _areaEntered = false;
	private bool _areaExited = false;
	private float _fadeVal = 0.0f;
	private string _curVideoFile;
	private List<MediaPlayer> _otherMediaPlayers = new List<MediaPlayer>();

	// Use this for initialization
	void Start () {

		fadeEffect.transform.SetParent(mainCamera.transform, false);

		videoSphere.SetActive (false);

		// append all media players in the scene other than 'mediaPlayer'
		foreach (var mp in FindObjectsOfType<MediaPlayer> ()) {
			if (mp != mediaPlayer)
				_otherMediaPlayers.Add (mp);
		}

	}

	private void UpdateVideoSpherePos() {
		videoSphere.transform.position = mainCamera.transform.position;
		videoSphere.transform.rotation = _curVideoSphereRotation.rotation;
	}

	private void Fade(Camera obj, bool fadeOut) {
		if (fadeOut) {
			_fadeVal += Time.deltaTime * fadeSpeed;
		} else {
			_fadeVal -= Time.deltaTime * fadeSpeed;
		}

		FadeEffect fadeEffect = obj.GetComponentInChildren<FadeEffect> ();
		fadeEffect.SetAlpha (_fadeVal);
	}

	private void LoadVideoFile ()
	{
		mediaPlayer.OpenVideoFromFile (MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, _curVideoFile);
		mediaPlayer.Control.SetVolume (0);
	}

	private void SwitchToVideoCamera ()
	{
		DemoApp app = FindObjectOfType<DemoApp> ();


		videoSphere.SetActive (true);


		// set culling mask
		_oldCullingMask = mainCamera.cullingMask;
		if (_curVideoSphereType == VideoSphereType.NormalVideoSphere) {
			
			mainCamera.cullingMask = 1 << LayerMask.NameToLayer ("VideoSphere")
				| 1 << LayerMask.NameToLayer ("TransparentFX");
			
		} else if (_curVideoSphereType == VideoSphereType.TransparentVideoSphere) {
			
			mainCamera.cullingMask |= 1 << LayerMask.NameToLayer ("VideoSphere")
				| 1 << LayerMask.NameToLayer ("TransparentFX");
		
		}


		// set transparency of the video sphere
		Material mat = videoSphere.GetComponent<Renderer> ().material;
		Color color = mat.GetColor ("_Color");
		if (_curVideoSphereType == VideoSphereType.NormalVideoSphere) {
			color.a = 1;

			mediaPlayer.m_AlphaPacking = AlphaPacking.None;

		} else if (_curVideoSphereType == VideoSphereType.TransparentVideoSphere) {
			color.a = 1;

			mediaPlayer.m_AlphaPacking = AlphaPacking.LeftRight;
		}
		mat.SetColor ("_Color", color);


		// play video file
		mediaPlayer.Play ();
	}

	private void SwitchToMainCamera ()
	{
		DemoApp app = FindObjectOfType<DemoApp> ();

		mainCamera.cullingMask = _oldCullingMask;

		videoSphere.SetActive (false);


		// stop playing video file
		mediaPlayer.Stop();
	}

	// Update is called once per frame
	void Update () {
		if (_status == Status.InMainCamera) {

			if (_areaEntered) {
				_areaEntered = false;
				_status = Status.MainCameraFadeOut;


				// load video file in advance
				LoadVideoFile ();
			}

		} else if (_status == Status.MainCameraFadeOut) {

			if (_fadeVal >= 1.0f) {
				SwitchToVideoCamera ();
				_status = Status.VideoCameraFadeIn;
			} else {
				Fade (mainCamera, true);

				// fade out audios in the main scene
				foreach (var mp in _otherMediaPlayers) {
					mp.Control.SetVolume (1.0f - _fadeVal);
				}
			}

		} else if (_status == Status.VideoCameraFadeIn) {

			UpdateVideoSpherePos ();

			if (_fadeVal <= 0.0f) {
				_status = Status.InVideoCamera;
			} else {
				Fade (mainCamera, false);

				// fade in audio channel in 360-video
				mediaPlayer.Control.SetVolume (1.0f - _fadeVal);
			}

		} else if (_status == Status.InVideoCamera) {

			UpdateVideoSpherePos ();

			if (_areaExited) {
				_areaExited = false;
				_status = Status.VideoCameraFadeOut;
			}

		} else if (_status == Status.VideoCameraFadeOut) {

			UpdateVideoSpherePos ();

			if (_fadeVal >= 1.0f) {
				SwitchToMainCamera ();
				_status = Status.MainCameraFadeIn;
			} else {
				Fade (mainCamera, true);

				// fade out audio channel in 360-video
				mediaPlayer.Control.SetVolume (1.0f - _fadeVal);
			}

		} else if (_status == Status.MainCameraFadeIn) {

			if (_fadeVal <= 0.0f) {
				_status = Status.InMainCamera;
			} else {
				Fade (mainCamera, false);

				// fade in audios in the main scene
				foreach (var mp in _otherMediaPlayers) {
					mp.Control.SetVolume (1.0f - _fadeVal);
				}
			}

		}

	}

	public void StartVideo(string fileName, VideoSphereType type, Transform videoSphereRotation) {
		_areaEntered = true;
		_curVideoFile = fileName;
		_curVideoSphereType = type;
		_curVideoSphereRotation = videoSphereRotation;
	}

	public void StopVideo() {
		_areaExited = true;
	}

}
