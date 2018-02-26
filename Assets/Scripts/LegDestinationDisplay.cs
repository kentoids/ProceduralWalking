using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegDestinationDisplay : MonoBehaviour {
	GameObject rTarget, lTarget, rGround, lGround;
	Mover mover;
	// Use this for initialization
	void Start () {
		if (gameObject.GetComponent<Mover>() == null) {
			this.enabled = false;
			return;
		}
		mover = gameObject.GetComponent<Mover>();

		rTarget = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		rTarget.transform.localScale = Vector3.one * 0.5f;
		Destroy(rTarget.GetComponent<SphereCollider>());
		rTarget.GetComponent<Renderer>().material.color = Color.red;

		lTarget = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		lTarget.transform.localScale = Vector3.one * 0.5f;
		Destroy(lTarget.GetComponent<SphereCollider>());
		lTarget.GetComponent<Renderer>().material.color = Color.blue;

		rGround = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		rGround.transform.localScale = Vector3.one * 0.5f;
		Destroy(rGround.GetComponent<SphereCollider>());
		rGround.GetComponent<Renderer>().material.color = Color.red;

		lGround = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		lGround.transform.localScale = Vector3.one * 0.5f;
		Destroy(lGround.GetComponent<SphereCollider>());
		lGround.GetComponent<Renderer>().material.color = Color.blue;
		// targetObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		// targetObj.transform.localScale = Vector3.one * 1f;
	}
	
	// Update is called once per frame
	void Update () {
		rTarget.GetComponent<Renderer>().material.color = Color.white;
			lTarget.GetComponent<Renderer>().material.color = Color.white;
			if (mover.rightMoving) rTarget.GetComponent<Renderer>().material.color = Color.red;
			else lTarget.GetComponent<Renderer>().material.color = Color.blue;
			rTarget.transform.position = mover.rDest;
			lTarget.transform.position = mover.lDest;
			rGround.transform.position = mover.rHit;
			lGround.transform.position = mover.lHit;
	}
}
