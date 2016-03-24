using UnityEngine;
using System.Collections;
using System;

public class ForcePreventFall : StiffnessControlledObjects {
	private ConstantForce theForce;
	private Vector3 resistForce;
	private float supportForce;
	public float springConstant = 162.55f;
	private Rigidbody weightInfo;

	// Use this for initialization
	void Start () {
		theForce = this.GetComponent<ConstantForce>();
		weightInfo = GameObject.Find("Weight").GetComponent<Rigidbody>();
		this.GetComponent<SpringJoint>().spring = springConstant;
		this.GetComponent<SpringJoint>().damper = 20;

		resistForce = theForce.relativeForce;
		supportForce = 8.7f;
		resistForce.y = supportForce;
		theForce.relativeForce = resistForce;
	}
	
	// Update is called once per frame
	void Update () {
		if (stiffness != previousStiffness)
		{
			currentTime = 0;
		}
		
		currentTime = Math.Max(1, (currentTime + 1)/60.0f);
	
		var tempForce = theForce.relativeForce;
		tempForce.y = Mathf.Lerp(theForce.relativeForce.y, supportForce + (100 * (stiffness - 1)), currentTime);
		theForce.relativeForce = tempForce;

		previousStiffness = stiffness;

	}
}
