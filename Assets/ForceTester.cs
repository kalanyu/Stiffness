using UnityEngine;
using System.Collections;
using System;

public class ForceTester : MonoBehaviour {
	ConstantForce theForce;
	public float stiffness;
	float resistForce;
	public float currentTime = 0;
	SpringJoint theSpring;
	public int direction = 1; //1 down 2 up 3 left 4 right
	// Use this for initialization
	void Start () {
		theForce = this.GetComponent<ConstantForce>();
		theSpring = this.GetComponent<SpringJoint>();
		
//		var tempForce = theForce.relativeForce;
//		resistForce = (this.GetComponent<Rigidbody>().mass - 1) * 10;
//		tempForce.y = resistForce;
//		theForce.relativeForce = tempForce;
		stiffness = 0;
//		theSpring.spring = 40.7f;
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey(KeyCode.UpArrow)) {
			var tempStiffness = stiffness + 0.05f;
			stiffness = Math.Min(1, tempStiffness);
			theSpring.damper = 10;
		}

		if (Input.GetKey(KeyCode.DownArrow)) {
			var tempStiffness = stiffness - 0.05f;
			stiffness = Math.Max(0, tempStiffness);
			theSpring.damper = 10;
		}

		currentTime = Math.Max(1, (currentTime + 1)/60.0f);
		var tempForce = theForce.relativeForce;
		tempForce.y = Mathf.Lerp(theForce.relativeForce.y, resistForce + (50 * stiffness) , currentTime);
		theForce.relativeForce = tempForce;
//		Debug.Log(theForce.force);
	}
}
