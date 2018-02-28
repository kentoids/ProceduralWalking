using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePositionWithKey : MonoBehaviour {
	public float strength = 1.0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.position += new Vector3(-Input.GetAxis("Horizontal") * strength, 0, -Input.GetAxis("Vertical") * strength);
	}
}
