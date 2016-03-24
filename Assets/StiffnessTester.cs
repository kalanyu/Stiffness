using UnityEngine;
using System.Collections;
using System;

public class StiffnessTester : MonoBehaviour {
	public float stiffness;
	public float springConstant;
    private SpringJoint slingJoint;
 	private float currentTime = 0.0f; 
	
	// Use this for initialization
	void Start () {
		slingJoint = this.transform.parent.transform.Find("hand").GetComponentInChildren<SpringJoint>();
		stiffness = 1;
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey(KeyCode.UpArrow)) {
			var tempStiffness = stiffness + 0.1f;
			stiffness = Math.Min(1, tempStiffness);
			slingJoint.damper = 15;
		}
		if (Input.GetKey(KeyCode.DownArrow)) {
			var tempStiffness = stiffness - 0.1f;
			stiffness = Math.Max(0, tempStiffness);
			slingJoint.damper = 7;
		}

		if (slingJoint != null) {
//			if (stiffness  <  0.9f)
//			{
				if (slingJoint.gameObject.GetComponent<Rigidbody>().IsSleeping()) {
					slingJoint.gameObject.GetComponent<Rigidbody>().WakeUp();
				}
				currentTime += 1.0f/60.0f;
				slingJoint.spring = Mathf.Lerp(springConstant, springConstant * stiffness, currentTime);
			if(currentTime == 1) {
				currentTime = 0;
			}
//			} else {
//				slingJoint.spring = 161.92f;
//				slingJoint.damper = 15;
//				currentTime = 0;
//			}
		}
	}
}
