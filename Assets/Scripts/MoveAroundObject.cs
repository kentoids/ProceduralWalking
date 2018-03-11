using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAroundObject : MonoBehaviour {
	public GameObject target;
	public bool moveWhenTouched = true;
	public float speed = 0.5f;
	float theta, radius;
	// Use this for initialization
	void Start () {
		// theta = 0;
		// Vector2 forTheta = new Vector2(transform.position.z, transform.position.x);
		// forTheta.Normalize();
		// Debug.Log("ratio: " + transform.position.z / transform.position.x);
		
		Vector3 toTarget = target.transform.position - gameObject.transform.position;
		toTarget.y = 0;
		radius = toTarget.magnitude;
		theta = Mathf.Atan2(transform.position.z, transform.position.x);
		if (theta > Mathf.PI * 2) theta = 0;
	}
	
	// Update is called once per frame
	void Update () {
		// moveWithArcLength(speed);
	}

	void OnTriggerStay(Collider other) {
		if (!moveWhenTouched) return;
		if (!other.transform.IsChildOf(transform.parent)) return;
		// Debug.Log("theta: " + theta);
		transform.position = new Vector3(Mathf.Cos(theta) * radius, transform.position.y, Mathf.Sin(theta) * radius);
		theta += speed / radius;
		if (theta > Mathf.PI * 2) theta = 0;
	}

	void moveWithArcLength(float arcLength) {
		transform.position = new Vector3(Mathf.Cos(theta) * radius, transform.position.y, Mathf.Sin(theta) * radius);
		theta += arcLength / radius;
		if (theta > Mathf.PI * 2) theta = 0;
	}

	void moveWithAngle(float _speed) {
		transform.position = new Vector3(Mathf.Cos(theta) * radius, transform.position.y, Mathf.Sin(theta) * radius);
		theta += _speed;
		if (theta > Mathf.PI * 2) theta = 0;
	}
}
