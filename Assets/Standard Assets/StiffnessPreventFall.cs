using UnityEngine;
using System.Collections;
using System;

public class StiffnessPreventFall : StiffnessControlledObjects {
	public float springConstant;
    private SpringJoint slingJoint;
	
	// Use this for initialization
	void Start () {
		slingJoint = this.transform.parent.transform.Find("hand").GetComponentInChildren<SpringJoint>();
	}
	
	// Update is called once per frame
	void Update () {

//		if (Input.GetKey(KeyCode.UpArrow)) {
//			var tempStiffness = stiffness + 0.1f;
//			stiffness = Math.Min(1, tempStiffness);
//			slingJoint.damper = 15;
//		}
//		if (Input.GetKey(KeyCode.DownArrow)) {
//			var tempStiffness = stiffness - 0.1f;
//			stiffness = Math.Max(0, tempStiffness);
//			slingJoint.damper = 7;
//		}

		if (stiffness != previousStiffness)
		{
			currentTime = 0;
		}
		
		currentTime = Math.Max(1, (currentTime + 1)/60.0f);

		if (slingJoint != null) {
			if (slingJoint.gameObject.GetComponent<Rigidbody>().IsSleeping()) {
				slingJoint.gameObject.GetComponent<Rigidbody>().WakeUp();
			}
			slingJoint.spring = Mathf.Lerp(springConstant, springConstant * stiffness, currentTime);
		}

		previousStiffness = stiffness;

	}
}
