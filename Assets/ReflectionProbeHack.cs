using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionProbeHack : MonoBehaviour {

	public float RenderReflectionProbeAfter = 1.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		RenderReflectionProbeAfter -= Time.deltaTime;
		if ( RenderReflectionProbeAfter < 0 )
		{
			GetComponent<ReflectionProbe> ().RenderProbe ();
		}

	}
}
