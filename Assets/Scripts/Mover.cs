using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour {
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

	public bool showTarget = true;
	public bool activateStabilizeRotation = true;
	// GameObject targetObj;
	
	public float speed = 1.0f;
	public float footThres = 0.1f;
	public float footStroke = 1.0f;
	public float headOffset = 0.5f;
	float footDistance;
	public bool rightMoving { get; private set;}
	Vector3 prevDirection = Vector3.forward;

	float startWalk = 0;

	// Use this for initialization
	void Start () {
		rightMoving = true;
		initHipPos = hip.transform.position;
		initRpos = rFoot.transform.position;
		initLpos = lFoot.transform.position;
		rDest = rFoot.transform.position;
		lDest = lFoot.transform.position;
		footDistance = (rFoot.transform.position - lFoot.transform.position).magnitude;
	}
	
	// Update is called once per frame
	void Update () {
		keepHeadAtCenter();
		moveTo(new Vector3(-Input.GetAxis("Horizontal"), 0, -Input.GetAxis("Vertical")));
		// stabilizeHead();
		if (activateStabilizeRotation) stabilizeRotation();
	}

	void keepHeadAtCenter() {
		Vector3 footCenter = rFoot.transform.position - lFoot.transform.position;
		footCenter /= 2;
		footCenter += lFoot.transform.position;
		Vector3 toTarget = footCenter - hip.transform.position;
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
		hip.GetComponent<Rigidbody>().AddForce(toTarget, ForceMode.VelocityChange);
	}

	void moveTo(Vector3 direction) {
		// check if destination is reached, if so update destination
		Vector3 curDest, curPos;
		if (rightMoving) {
			curDest = rDest;
			curPos = rFoot.transform.position;
		} else {
			curDest = lDest;
			curPos = lFoot.transform.position;
		}

		if ((curDest - curPos).magnitude < footThres) {
			// right destination reached, update left destination
			if (rightMoving) rightMoving = false;
			else rightMoving = true;

			direction.Normalize();
			direction *= footStroke;
			Vector3 newDestination;
			if (direction.sqrMagnitude == 0) {
				newDestination = Vector3.Cross(prevDirection.normalized, Vector3.up);
			} else {
				newDestination = Vector3.Cross(direction.normalized, Vector3.up);
				prevDirection = direction.normalized;
			}
			newDestination *= footDistance/2;
			if (rightMoving) newDestination *= -1;
			if (direction.sqrMagnitude != 0) newDestination += direction;
			
			if (rightMoving) {
				// add function to check next height
				// if too high, don't update destination

				Vector3 checkGroundFor = hip.transform.position + newDestination;
				Ray ray = new Ray(checkGroundFor, Vector3.down);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, 10.0f)) {
					// update destination if there is ground
					rDest = hip.transform.position + newDestination;
					Debug.Log("hit at " + hit.collider.gameObject);
					Debug.Log("<color=red>hit.y: </color>" + hit.point.y);
					// rDest.y = hit.point.y + initRpos.y;
					rDest = new Vector3(rDest.x, hit.point.y + initRpos.y, rDest.z);
					rHit = hit.point;
					curDest = rDest;
				}
				Debug.DrawRay(ray.origin, ray.direction, Color.red, 3.0f);

				// rDest = hip.transform.position + newDestination;
				// rDest.y = rFoot.transform.position.y;
				// curDest = rDest;
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
					Debug.Log("hit at " + hit.collider.gameObject);
					Debug.Log("<color=red>hit.y: </color>" + hit.point.y);
					curDest = lDest;
				}
				Debug.DrawRay(ray.origin, ray.direction, Color.red, 3.0f);

				// lDest = hip.transform.position + newDestination;
				// lDest.y = lFoot.transform.position.y;
				// curDest = lDest;
			}
			
		}

		// if both foot is in place, return
		if (direction.sqrMagnitude == 0) {
			if ((rDest - rFoot.transform.position).magnitude < footThres &&
				(lDest - lFoot.transform.position).magnitude < footThres) {
					return;
			}
		}

		Vector3 newMove;
		newMove = curDest - curPos;
		if (newMove.magnitude > 1) newMove.Normalize();
		newMove *= speed;

		if (rightMoving) {
			rFoot.GetComponent<Rigidbody>().AddForce(newMove, ForceMode.VelocityChange);
		} else {
			lFoot.GetComponent<Rigidbody>().AddForce(newMove, ForceMode.VelocityChange);
		}
	}

	void stabilizeRotation() {
		Vector2 normHip = new Vector2(hip.transform.forward.normalized.x, hip.transform.forward.normalized.z);
		Vector2 toAngle = new Vector2(prevDirection.x, prevDirection.z) - normHip;
		float toTargetAngle = Vector2.SignedAngle(new Vector2(prevDirection.x, prevDirection.z), normHip);

		Debug.DrawLine(hip.transform.position, hip.transform.position + new Vector3(prevDirection.x, 0, prevDirection.z) * 10, Color.blue, 0, false);
		Debug.DrawLine(hip.transform.position, hip.transform.position + new Vector3(normHip.x, 0, normHip.y) * 10, Color.green, 0, false);
		Debug.DrawLine(hip.transform.position, hip.transform.position + new Vector3(toAngle.x, 0, toAngle.y) * 10, Color.red, 0, false);
		// print("toTargetAngle: " + toTargetAngle);
		// print("toAngle: " + toAngle);

		if (Mathf.Abs(toTargetAngle) > angleThres) {
			if (toTargetAngle > 0) toTargetAngle = anglePower;
			else toTargetAngle = -anglePower;
		} else {
			toTargetAngle /= angleDivision;
		}
		// print("edited toTargetAngle: " + toTargetAngle);

		hip.GetComponent<Rigidbody>().AddTorque(Vector3.up * toTargetAngle, ForceMode.VelocityChange);
	}

	void stabilizeRotation_() {
		Vector3 toAngle = prevDirection - hip.transform.forward;
		// print("toAngle: " + toAngle.magnitude);
		// toAngle.Normalize();
		// toAngle *= anglePower;

		Debug.DrawLine(hip.transform.position, hip.transform.position + prevDirection * 10, Color.blue, 0, false);
		Debug.DrawLine(hip.transform.position, hip.transform.position + hip.transform.forward * 10, Color.green, 0, false);
		Debug.DrawLine(hip.transform.position, hip.transform.position + toAngle * 10, Color.red, 0, false);

		if (toAngle.magnitude > angleThres) {
			toAngle.Normalize();
			toAngle *= anglePower;
			// print("OUT: " + toAngle.magnitude);
		} else {
			toAngle /= angleDivision;
			// print("IN: " + toAngle.magnitude);
		}

		hip.transform.LookAt(hip.transform.forward + toAngle);
		// hip.GetComponent<Rigidbody>().AddTorque(toAngle, ForceMode.VelocityChange);
		// GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(transform.forward + toAngle/100));
	}
}
