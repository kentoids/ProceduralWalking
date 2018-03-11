using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour {
	public GameObject mainTarget;
	public GameObject hip, rFoot, lFoot;
	Vector3 initHipPos, initRpos, initLpos;
	public Vector3 rHit { get; private set;}
	public Vector3 lHit { get; private set;}
	public Vector3 rDest { get; private set;}
	public Vector3 lDest { get; private set;}
	public float power = 1;
	public float distThres = 1.0f;
	public float anglePower = 1;
	public float angleThres = 1.0f;
	public float angleDivision = 50f;

	public bool showdebug = false;
	public bool activateStabilizeRotation = true;
	
	public float speedPerSecond = 1.0f;
	public float speedVariance = 100f;
	public float footThres = 0.1f;
	public float footStroke = 1.0f;
	public float footStrokeVariance = 0.1f;
	public float headOffset = 0.5f;
	float footDistance;
	public bool rightMoving { get; private set;}
	Vector3 prevDirection = Vector3.forward;

	float startWalk = 0;

	// Use this for initialization
	void Start () {
		rightMoving = true;
		initRpos = rFoot.transform.position;
		initLpos = lFoot.transform.position;
		initHipPos = hip.transform.position;
		// have to change initHipPos to represent distance between ankle for climbing slopes
		// initHipPos.y = initHipPos.y - initRpos.y;
		rDest = rFoot.transform.position;
		lDest = lFoot.transform.position;
		footDistance = (rFoot.transform.position - lFoot.transform.position).magnitude;

		speedPerSecond += Random.Range(-speedVariance, speedVariance);
		footStroke += Random.Range(-footStrokeVariance, footStrokeVariance);
	}
	
	// Update is called once per frame
	void Update () {
		keepHeadAtCenter();
		Vector3 toMainTarget = mainTarget.transform.position - hip.transform.position;
		toMainTarget.y = 0;
		moveToTarget(toMainTarget);
		// moveTo(new Vector3(-Input.GetAxis("Horizontal"), 0, -Input.GetAxis("Vertical")));
		// stabilizeHead();
		if (activateStabilizeRotation) stabilizeRotation();
	}

	void keepHeadAtCenter() {
		Vector3 footCenter = rFoot.transform.position - lFoot.transform.position;
		footCenter /= 2;
		footCenter += lFoot.transform.position;
		Vector3 toTarget = footCenter - hip.transform.position;
		// toTarget.y = initHipPos.y + (rFoot.transform.position.y + lFoot.transform.position.y)/2 + headOffset - hip.transform.position.y;
		toTarget.y = initHipPos.y + headOffset - hip.transform.position.y;
		if (toTarget.magnitude > distThres) {
			toTarget.Normalize();
			toTarget *= power;
		} else {
			toTarget *= power;
			if (toTarget.magnitude > power / 2) {
				toTarget.Normalize();
				toTarget *= power / 2.0f;
			}
		}
		toTarget *= Time.deltaTime;
		hip.GetComponent<Rigidbody>().AddForce(toTarget, ForceMode.VelocityChange);
	}

	void moveToTarget(Vector3 targetPos) {
		// check if destination is reached, if so update destination
		Vector3 curDest, curPos;
		if (rightMoving) {
			curDest = rDest;
			curPos = rFoot.transform.position;
		} else {
			curDest = lDest;
			curPos = lFoot.transform.position;
		}
		Vector3 orgPos = targetPos;

		// check if movign foot should be changed
		if ((curDest - curPos).magnitude < footThres) {
			// right destination reached, update left destination
			rightMoving = !rightMoving;

			// new destination for moving foot
			targetPos.Normalize();
			targetPos *= footStroke;
			Vector3 newDestination;
			if (targetPos.sqrMagnitude == 0) {
				newDestination = Vector3.Cross(prevDirection.normalized, Vector3.up);
			} else {
				// note: check here if destination goes wrong. getting the cross product of slanted vector may result weird results
				newDestination = Vector3.Cross(targetPos.normalized, Vector3.up);
				prevDirection = targetPos.normalized;
			}
			newDestination *= footDistance/2;
			if (rightMoving) newDestination *= -1;
			if (targetPos.sqrMagnitude != 0) newDestination += targetPos;
			
			if (rightMoving) {
				// add function to check next height
				// if too high, don't update destination

				Vector3 checkGroundFor = hip.transform.position + newDestination;
				Ray ray = new Ray(checkGroundFor, Vector3.down);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, 10.0f)) {
					// update destination if there is ground
					rDest = hip.transform.position + newDestination;
					// rDest.y = hit.point.y + initRpos.y;
					rDest = new Vector3(rDest.x, hit.point.y + initRpos.y, rDest.z);
					rHit = hit.point;
					// Debug.Log("hit at " + hit.collider.gameObject);
					// Debug.Log("<color=red>hit.y: </color>" + hit.point.y);
					curDest = rDest;
				}
				if (showdebug) Debug.DrawRay(ray.origin, ray.direction, Color.red, 3.0f);
			} else {
				Vector3 checkGroundFor = hip.transform.position + newDestination;
				Ray ray = new Ray(checkGroundFor, Vector3.down);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, 10.0f)) {
					// update destination if there is ground
					lDest = hip.transform.position + newDestination;
					// lDest.y = hit.point.y + initLpos.y;
					lDest = new Vector3(lDest.x, hit.point.y + initRpos.y, lDest.z);
					lHit = hit.point;
					// Debug.Log("hit at " + hit.collider.gameObject);
					// Debug.Log("<color=red>hit.y: </color>" + hit.point.y);
					curDest = lDest;
				}
				if (showdebug) Debug.DrawRay(ray.origin, ray.direction, Color.red, 3.0f);
			}
			// if (showdebug) Debug.Log("curDest: " + curDest);
			
		}

		// if both foot is in place, return
		if (orgPos.magnitude < footThres) {
			if ((rDest - rFoot.transform.position).magnitude < footThres * 2 &&
				(lDest - lFoot.transform.position).magnitude < footThres * 2) {
					return;
			}
		}

		Vector3 newMove;
		newMove = curDest - curPos;
		if (newMove.magnitude > 1) newMove.Normalize();
		newMove *= speedPerSecond;
		newMove *= Time.deltaTime;

		if (rightMoving) {
			rFoot.GetComponent<Rigidbody>().AddForce(newMove, ForceMode.Acceleration);
		} else {
			lFoot.GetComponent<Rigidbody>().AddForce(newMove, ForceMode.Acceleration);
		}
	}

	void stabilizeRotation() {
		Vector2 normHip = new Vector2(hip.transform.forward.normalized.x, hip.transform.forward.normalized.z);
		Vector2 toAngle = new Vector2(prevDirection.x, prevDirection.z) - normHip;
		float toTargetAngle = Vector2.SignedAngle(new Vector2(prevDirection.x, prevDirection.z), normHip);

		Debug.DrawLine(hip.transform.position, hip.transform.position + new Vector3(prevDirection.x, 0, prevDirection.z) * 10, Color.blue, 0, false);
		Debug.DrawLine(hip.transform.position, hip.transform.position + new Vector3(normHip.x, 0, normHip.y) * 10, Color.green, 0, false);
		Debug.DrawLine(hip.transform.position, hip.transform.position + new Vector3(toAngle.x, 0, toAngle.y) * 10, Color.red, 0, false);

		if (Mathf.Abs(toTargetAngle) > angleThres) {
			if (toTargetAngle > 0) toTargetAngle = anglePower;
			else toTargetAngle = -anglePower;
		} else {
			toTargetAngle /= angleDivision;
		}

		hip.GetComponent<Rigidbody>().AddTorque(Vector3.up * toTargetAngle, ForceMode.VelocityChange);
	}
}
