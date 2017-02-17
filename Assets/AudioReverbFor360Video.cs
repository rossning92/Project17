using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioReverbFor360Video : MonoBehaviour {


	public enum ReverbTransitionCurve
	{
		Linear, Logarithmic, Exponential, Sigmoid
	}


	public AudioReverbFilter audioReverb0Degree;
	public AudioReverbFilter audioReverb180Degree;
	public ReverbTransitionCurve reverbTransitionCurve = ReverbTransitionCurve.Linear;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (GetComponent<VideoTrigger> ().IsTriggerActive ()) {

			float t = DemoApp.Instance.GetRotationYInVideoSphere ();

			// convert to -180 ~ 180 degree
			if (t > 180.0f) t -= 360.0f;

			// narrow down to 0 ~ 180 degree
			t = Mathf.Abs(t);

			// normalize to 0 ~ 1
			t /= 180.0f;
		

			if (reverbTransitionCurve == ReverbTransitionCurve.Exponential) {
				t = Mathf.Sin (1.5f * Mathf.PI + 0.5f * Mathf.PI * t) + 1.0f;
			} else if (reverbTransitionCurve == ReverbTransitionCurve.Logarithmic) {
				t = Mathf.Sin (0.5f * Mathf.PI * t);
			} else if (reverbTransitionCurve == ReverbTransitionCurve.Sigmoid) {
				t = 0.5f * Mathf.Cos (Mathf.PI + Mathf.PI * t) + 0.5f;
			}


			LerpReverbEffect (DemoApp.Instance.videoSphereReverbFilter, audioReverb0Degree, audioReverb180Degree, t);

		}
	}

	void LerpReverbEffect (AudioReverbFilter target, AudioReverbFilter a, AudioReverbFilter b, float t)
	{
		target.decayHFRatio = Mathf.Lerp (a.decayHFRatio, b.decayHFRatio, t);
		target.decayTime = Mathf.Lerp (a.decayTime, b.decayTime, t);
		target.density = Mathf.Lerp (a.density, b.density, t);
		target.diffusion = Mathf.Lerp (a.diffusion, b.diffusion, t);
		target.dryLevel = Mathf.Lerp (a.dryLevel, b.dryLevel, t);
		target.hfReference = Mathf.Lerp (a.hfReference, b.hfReference, t);
		target.lfReference = Mathf.Lerp (a.lfReference, b.lfReference, t);
		target.reflectionsDelay = Mathf.Lerp (a.reflectionsDelay, b.reflectionsDelay, t);
		target.reflectionsLevel = Mathf.Lerp (a.reflectionsLevel, b.reflectionsLevel, t);
		target.reverbDelay = Mathf.Lerp (a.reverbDelay, b.reverbDelay, t);
		target.reverbLevel = Mathf.Lerp (a.reverbLevel, b.reverbLevel, t);
		target.room = Mathf.Lerp (a.room, b.room, t);
		target.roomHF = Mathf.Lerp (a.roomHF, b.roomHF, t);
		target.roomLF = Mathf.Lerp (a.roomLF, b.roomLF, t);
		target.roomRolloff = Mathf.Lerp (a.roomRolloff, b.roomRolloff, t);
	}
}
