using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAroundObject : MonoBehaviour {
	public GameObject target;
	public float speed = 0.5f;
	float theta, radius;
	// Use this for initialization
	void Start () {
		theta = 0;
		Vector3 toTarget = target.transform.position - gameObject.transform.position;
		toTarget.y = 0;
		radius = toTarget.magnitude;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(Mathf.Cos(theta) * radius, transform.position.y, Mathf.Sin(theta) * radius);
		theta += speed;
		if (theta > Mathf.PI * 2) theta = 0;
	}
}
