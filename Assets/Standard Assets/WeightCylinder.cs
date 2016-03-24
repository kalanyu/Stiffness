using UnityEngine;
using System;
using System.Collections;

public class WeightCylinder : StiffnessControlledObjects {

	private SpringJoint slingJoint;
	public bool collided = false;

	// Use this for initialization
	void Start () {
		slingJoint = this.GetComponent<SpringJoint>();
		stiffness = 0.0f;
	}
	
	// Update is called once per frame

	void Update () {
//		if (gameObject.GetComponent<Rigidbody> ().IsSleeping() && !printed) {
//			printed = true;
//		}

//		if (Input.GetKey(KeyCode.UpArrow)) {
//			var tempStiffness = stiffness + 0.1f;
//			stiffness = Math.Min(1, tempStiffness);
//		}
//
//		if (Input.GetKey(KeyCode.DownArrow)) {
//			var tempStiffness = stiffness - 0.1f;
//			stiffness = Math.Max(0, tempStiffness);
//		}
		
		if (stiffness != previousStiffness)
		{
			currentTime = 0;
		}
		
		currentTime = Math.Max(1, (currentTime + 1)/60.0f);

		if (stiffness > 0.1f)
		{
			if (slingJoint.gameObject.GetComponent<Rigidbody>().IsSleeping()) {
				slingJoint.gameObject.GetComponent<Rigidbody>().WakeUp();
			}
			slingJoint.spring = Mathf.Lerp(0, 40.7f * stiffness, currentTime);
			//tests if low or high damper has any effect
			slingJoint.damper = 7;
		} else {
			slingJoint.spring = 0;
			slingJoint.damper = 0;
		}

		previousStiffness = stiffness;

	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.name == "Placeholder") {
			Debug.Log("collided");
			collided = true;
			var weightPos = this.transform.position;
			var cubeTransform = this.transform.parent.transform.Find("Cube").transform.position;
			weightPos.y -= 0.128f;
			weightPos.x = cubeTransform.x;
			weightPos.z = cubeTransform.z;
			transform.parent.transform.Find("Cube").transform.position = weightPos;
			transform.parent.transform.Find("Cube").transform.parent = this.transform;
		}
	}
}
