using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixSpherePosToVRCamera : MonoBehaviour {

	public GameObject cameraNode;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.localPosition = cameraNode.transform.localPosition;
	}
}
