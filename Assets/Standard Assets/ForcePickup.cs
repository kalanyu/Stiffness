using UnityEngine;
using System.Collections;
using System;

public class ForcePickup : StiffnessControlledObjects {
	private ConstantForce theForce;
	private Vector3 resistForce;
	private float supportForce;
	private Rigidbody weightInfo;

	// Use this for initialization
	void Awake() {
		stiffness = 0.0f;
		theForce = this.GetComponent<ConstantForce>();
		weightInfo = GameObject.Find("Weight").GetComponent<Rigidbody>();
		resistForce = theForce.relativeForce;
		supportForce = 9.8f;
		resistForce.y = supportForce;
		theForce.relativeForce = resistForce;
	}

	public void resetSupportForce() {
			this.supportForce = this.weightInfo.mass * 9.8f;
	}	
	// Update is called once per frame
	void Update () {
//		if (Input.GetKey(KeyCode.UpArrow)) {
//			var tempStiffness = stiffness + 0.05f;
//			stiffness = Math.Min(1, tempStiffness);
//			
//		}
//
//		if (Input.GetKey(KeyCode.DownArrow)) {
//			var tempStiffness = stiffness - 0.05f;
//			stiffness = Math.Max(0, tempStiffness);
//		}
	
	
		if (stiffness != previousStiffness)
		{
			currentTime = 0;
		}

		currentTime = Math.Max(1, (currentTime + 1)/60.0f);
		currentTime = Math.Max(1, (currentTime + 1)/60.0f);
		var tempForce = theForce.relativeForce;
		tempForce.y = Mathf.Lerp(theForce.relativeForce.y, 9.8f + 70 * stiffness, currentTime);
		theForce.relativeForce = tempForce;

		previousStiffness = stiffness;

	}
}
