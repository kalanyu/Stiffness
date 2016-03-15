using UnityEngine;
using System.Collections;
using System;

public class ForceReleaseTester : MonoBehaviour {
	ConstantForce theForce;
	public float stiffness = 1.0f;
	public float currentTime = 0;
	public float resistForce;
	SpringJoint theSpring;
	public int direction = 1; //1 down 2 up 3 left 4 right
	// Use this for initialization
	void Start () {
		theForce = this.GetComponent<ConstantForce>();
		theSpring = this.GetComponent<SpringJoint>();

		var tempForce = theForce.relativeForce;
		resistForce = this.GetComponent<Rigidbody>().mass * 10;
		tempForce.y = resistForce;
		theForce.relativeForce = tempForce;
		stiffness = 1;
//		Debug.Log(theForce.relativeForce);
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey(KeyCode.UpArrow)) {
			var tempStiffness = stiffness + 0.05f;
			stiffness = Math.Min(1, tempStiffness);
			theSpring.damper = 15;
			currentTime = 0;
		}

		if (Input.GetKey(KeyCode.DownArrow)) {
			var tempStiffness = stiffness - 0.05f;
			stiffness = Math.Max(0, tempStiffness);
			theSpring.damper = 10;
			currentTime = 0;
		}

		currentTime = Math.Max(1, (currentTime + 1)/60.0f);
		var tempForce = theForce.relativeForce;
		tempForce.y = Mathf.Lerp(theForce.relativeForce.y, resistForce * stiffness, currentTime);
		theForce.relativeForce = tempForce;

//		Debug.Log(theForce.force);
	}
}
