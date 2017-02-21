using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionProbeHack : MonoBehaviour {

	public float RenderReflectionProbeAfter = 1.0f;

	private float _countDown;
	private bool _rendered = false;


	// Use this for initialization
	void Start () {
		_countDown = RenderReflectionProbeAfter;
	}
	
	// Update is called once per frame
	void Update () {

		if ( !_rendered )
		{
			if (_countDown < 0) {
				GetComponent<ReflectionProbe> ().RenderProbe ();
				_rendered = true;
			} else {
				_countDown -= Time.deltaTime;
			}
		}

	}
}
