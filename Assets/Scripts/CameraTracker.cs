using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracker : MonoBehaviour {
	public GameObject trackTarget;
	Vector3 offset;
	// Use this for initialization
	void Start () {
		offset = trackTarget.transform.position - transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = trackTarget.transform.position - offset;
		// transform.LookAt(trackTarget.transform.position);
	}
}
