using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeEffect : MonoBehaviour {

	private Material _material;

	void Start () {
		_material = GetComponent<Renderer> ().material;
		SetAlpha (0);
	}

	public void SetAlpha(float alpha) {
		_material.SetColor ("_Color", new Color (0, 0, 0, alpha));
	}
}
