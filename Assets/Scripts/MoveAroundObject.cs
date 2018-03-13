using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAroundObject : MonoBehaviour {
	public GameObject target;
	public bool moveWhenTouched = true;
	public float speed = 0.5f;
	public bool avoide = false;
	float lookDistance = 3f;
	public float avoidStrength = 0.5f;
	Vector3 velocity, nextpos;
	float theta, radius;
	bool touchingTarget = false;
	bool stopForCollision = false;
	bool doDrawingStuff = false;
	Vector3 direction = Vector3.zero;

	Renderer ballrend;
	// Use this for initialization
	void Start () {
		// theta = 0;
		// Vector2 forTheta = new Vector2(transform.position.z, transform.position.x);
		// forTheta.Normalize();
		// Debug.Log("ratio: " + transform.position.z / transform.position.x);
		if ((ballrend = GetComponent<MeshRenderer>()).enabled) doDrawingStuff = true;
		
		Vector3 toTarget = target.transform.position - gameObject.transform.position;
		toTarget.y = 0;
		radius = toTarget.magnitude;
		theta = Mathf.Atan2(transform.position.z, transform.position.x);
		if (theta > Mathf.PI * 2) theta = 0;
	}
	
	// Update is called once per frame
	void Update () {
		moveByTouch();
		if (touchingTarget && !stopForCollision) {
			if (avoide) transform.position += velocity + checkForCollision();
			else transform.position += velocity;
		}
		stopForCollision = false;
		touchingTarget = false;
	}

	void OnTriggerStay(Collider other) {
		if (!moveWhenTouched) return;
		if (!other.transform.IsChildOf(transform.parent)) {
			stopForCollision = true;
			return;
		}
		touchingTarget = true;
		// transform.position = new Vector3(Mathf.Cos(theta) * radius, transform.position.y, Mathf.Sin(theta) * radius);
		// theta += speed / radius;
		// if (theta > Mathf.PI * 2) theta = 0;
	}

	Vector3 checkForCollision() {
		Ray ray = new Ray(transform.position, velocity.normalized);
		RaycastHit hit;
		Vector3 avoidance = Vector3.zero;
		if (Physics.Raycast(ray, out hit, lookDistance)) {
			avoidance = hit.transform.position - (transform.position + velocity.normalized * lookDistance);
			avoidance.Normalize();
			avoidance *= avoidStrength;
			ballrend.material.color = Color.green;
		}
		Debug.DrawRay(ray.origin, ray.direction * lookDistance, Color.red, 0f);
		return avoidance;
	}

	void moveByTouch() {
		// if not touching someone and touching agent target
		if (touchingTarget && !stopForCollision) {
			// nextpos = new Vector3(Mathf.Cos(theta) * radius, transform.position.y, Mathf.Sin(theta) * radius);
			velocity = new Vector3(Mathf.Cos(theta) * radius, transform.position.y, Mathf.Sin(theta) * radius) - transform.position;
			// velocity = new Vector3(Mathf.Cos(theta) * radius, transform.position.y, Mathf.Sin(theta) * radius);
			theta += speed / radius;
			if (theta > Mathf.PI * 2) theta = 0;
		}
		if (doDrawingStuff) {
			if (stopForCollision) GetComponent<Renderer>().material.color = Color.yellow;
			else GetComponent<Renderer>().material.color = Color.white;
		}
	}

	void moveByTouch_() {
		// if not touching someone and touching agent target
		if (touchingTarget && !stopForCollision) {
			transform.position = new Vector3(Mathf.Cos(theta) * radius, transform.position.y, Mathf.Sin(theta) * radius);
			theta += speed / radius;
			if (theta > Mathf.PI * 2) theta = 0;
		}
		if (doDrawingStuff) {
			if (stopForCollision) GetComponent<Renderer>().material.color = Color.yellow;
			else GetComponent<Renderer>().material.color = Color.white;
		}
		stopForCollision = false;
		touchingTarget = false;
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
