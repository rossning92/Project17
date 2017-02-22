using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;

[DisallowMultipleComponent]
public class DemoApp : MonoBehaviour {

	public static DemoApp Instance;

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
	public GameObject viveCamera;
	public AudioReverbFilter videoSphereReverbFilter;


	private int _oldCullingMask;
	private float _initVideoSphereRotY;
	private Status _status = Status.InMainCamera;
	private bool _areaEntered = false;
	private bool _areaExited = false;
	private float _fadeVal = 0.0f;
	private List<MediaPlayer> _otherMediaPlayers = new List<MediaPlayer>();
	private VideoTrigger _currentVideoTrigger;
	private Dictionary<VideoTrigger, float> _lastVideoPosMs = new Dictionary<VideoTrigger, float>();
	private Dictionary<VideoTrigger, float> _restartTimeInSecs = new Dictionary<VideoTrigger, float>();
	private bool _seekFinished = false;


	// Use this for initialization
	void Start () {

		Instance = this;

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

		videoSphere.transform.rotation = new Quaternion ();
		videoSphere.transform.Rotate (0, _initVideoSphereRotY, 0);
		videoSphere.transform.Rotate (_currentVideoTrigger.videoSphereRotation.rotation.eulerAngles);
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
		string fileName = _currentVideoTrigger.videoFile;
		mediaPlayer.OpenVideoFromFile (MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, fileName, false);
		mediaPlayer.Control.SetVolume (0);
		_seekFinished = false;
	}

	private void SwitchToVideoCamera ()
	{
		DemoApp app = FindObjectOfType<DemoApp> ();


		videoSphere.SetActive (true);

		VideoSphereType sphereType = _currentVideoTrigger.videoSphereType;

		// set culling mask
		_oldCullingMask = mainCamera.cullingMask;
		if (sphereType == VideoSphereType.NormalVideoSphere) {
			
			mainCamera.cullingMask = 
				1 << LayerMask.NameToLayer ("VideoSphere") |
				1 << LayerMask.NameToLayer ("TransparentFX");
			
		} else if (sphereType == VideoSphereType.TransparentVideoSphere) {
			
			mainCamera.cullingMask |= 
				1 << LayerMask.NameToLayer ("VideoSphere") |
				1 << LayerMask.NameToLayer ("TransparentFX");
		
		} else {

			// only video sphere and FX are visible
			mainCamera.cullingMask = 
				1 << LayerMask.NameToLayer ("VideoSphere") |
				1 << LayerMask.NameToLayer ("TransparentFX");

		}


		// set transparency of the video sphere
		Material mat = videoSphere.GetComponent<Renderer> ().material;
		Color color = mat.GetColor ("_Color");
		if (sphereType == VideoSphereType.NormalVideoSphere) {

			color.a = 1;
			mediaPlayer.m_AlphaPacking = AlphaPacking.None;

		} else if (sphereType == VideoSphereType.TransparentVideoSphere) {

			color.a = 1;
			mediaPlayer.m_AlphaPacking = AlphaPacking.LeftRight;

		} else {

			color.a = 1;
			mediaPlayer.m_AlphaPacking = AlphaPacking.None;

		}
		mat.SetColor ("_Color", color);


		// get current camera rotation when entering video sphere 
		_initVideoSphereRotY = mainCamera.transform.rotation.eulerAngles.y;


		if (sphereType == VideoSphereType.VideoSphereWithViveCamera) {
			viveCamera.SetActive (true);
		}


		// play video file
		mediaPlayer.Play ();
		_seekFinished = false;
	}

	private void SwitchToMainCamera ()
	{
		DemoApp app = FindObjectOfType<DemoApp> ();

		mainCamera.cullingMask = _oldCullingMask;


		videoSphere.SetActive (false);
		viveCamera.SetActive (false);


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

			TrySeekVideo ();

			UpdateVideoSpherePos ();

			if (_fadeVal <= 0.0f) {
				_status = Status.InVideoCamera;
			} else {
				Fade (mainCamera, false);

				// fade in audio channel in 360-video
				mediaPlayer.Control.SetVolume (1.0f - _fadeVal);
			}

		} else if (_status == Status.InVideoCamera) {

			TrySeekVideo ();

			UpdateVideoSpherePos ();

			if (_areaExited) {
				_areaExited = false;
				_status = Status.VideoCameraFadeOut;

				// save current video pos
				float t = mediaPlayer.Control.GetCurrentTimeMs();
				_lastVideoPosMs [_currentVideoTrigger] = t;

				t = _currentVideoTrigger.RestartVideoAfterSecs;
				if (t > 0) {
					_restartTimeInSecs [_currentVideoTrigger] = Time.time + t;
				}
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

	public void StartVideo(VideoTrigger videoTrigger) {
		_areaEntered = true;
		_currentVideoTrigger = videoTrigger;
	}

	public void StopVideo() {
		_areaExited = true;
	}

	public VideoTrigger GetCurrentVideoTrigger() {
		return _currentVideoTrigger;
	}

	public float GetRotationYInVideoSphere() {
		float rotY = mainCamera.transform.rotation.eulerAngles.y - _initVideoSphereRotY;
		while (rotY < 0)
			rotY += 360.0f;
		return rotY;
	}

	private void TrySeekVideo() {
		if (!_seekFinished && mediaPlayer.Control.IsPlaying()) {

			// Check if we need to resume the video
			float t = _restartTimeInSecs.ContainsKey (_currentVideoTrigger) ? _restartTimeInSecs [_currentVideoTrigger] : 0;
			if (t > Time.time) {
				mediaPlayer.Control.Seek (_lastVideoPosMs [_currentVideoTrigger]);
				Debug.Log ("SeekTo: " + _lastVideoPosMs [_currentVideoTrigger]);
				_seekFinished = true;
			}

		}
	}
}
