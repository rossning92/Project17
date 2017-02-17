using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioReverbFor360Video : MonoBehaviour {

	public AudioReverbFilter audioReverb0Degree;
	public AudioReverbFilter audioReverb180Degree;

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
		

			AudioReverbFilter f = DemoApp.Instance.videoSphereReverbFilter;

			f.decayHFRatio = Mathf.Lerp (audioReverb0Degree.decayHFRatio, audioReverb180Degree.decayHFRatio, t);
			f.decayTime = Mathf.Lerp (audioReverb0Degree.decayTime, audioReverb180Degree.decayTime, t);
			f.density = Mathf.Lerp (audioReverb0Degree.density, audioReverb180Degree.density, t);
			f.diffusion = Mathf.Lerp (audioReverb0Degree.diffusion, audioReverb180Degree.diffusion, t);
			f.dryLevel = Mathf.Lerp (audioReverb0Degree.dryLevel, audioReverb180Degree.dryLevel, t);
			f.hfReference = Mathf.Lerp (audioReverb0Degree.hfReference, audioReverb180Degree.hfReference, t);
			f.lfReference = Mathf.Lerp (audioReverb0Degree.lfReference, audioReverb180Degree.lfReference, t);
			f.reflectionsDelay = Mathf.Lerp (audioReverb0Degree.reflectionsDelay, audioReverb180Degree.reflectionsDelay, t);
			f.reflectionsLevel = Mathf.Lerp (audioReverb0Degree.reflectionsLevel, audioReverb180Degree.reflectionsLevel, t);
			f.reverbDelay = Mathf.Lerp (audioReverb0Degree.reverbDelay, audioReverb180Degree.reverbDelay, t);
			f.reverbLevel = Mathf.Lerp (audioReverb0Degree.reverbLevel, audioReverb180Degree.reverbLevel, t);
			f.room = Mathf.Lerp (audioReverb0Degree.room, audioReverb180Degree.room, t);
			f.roomHF = Mathf.Lerp (audioReverb0Degree.roomHF, audioReverb180Degree.roomHF, t);
			f.roomLF = Mathf.Lerp (audioReverb0Degree.roomLF, audioReverb180Degree.roomLF, t);
			f.roomRolloff = Mathf.Lerp (audioReverb0Degree.roomRolloff, audioReverb180Degree.roomRolloff, t);

		}
	}
}
