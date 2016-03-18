using UnityEngine;
using System.Collections;
using System;

public class ForcePickup : MonoBehaviour {
	private ConstantForce theForce;
	private Vector3 resistForce;
	private float supportForce;
	public float stiffness = 0.0f;
	private float previousStiffness = 0.0f;

	private float currentTime = 0.0f;
	// Use this for initialization
	void Start () {
		theForce = this.GetComponent<ConstantForce>();
		resistForce = theForce.relativeForce;
		supportForce = GameObject.Find("Weight").GetComponent<Rigidbody>().mass * 9.8f;
		resistForce.y = supportForce;
		theForce.relativeForce = resistForce;
	}
	
	public void resetSupportForce() {
		supportForce = GameObject.Find("Weight").GetComponent<Rigidbody>().mass * 9.8f;
//		Debug.Log(GameObject.Find("Weight").GetComponent<Rigidbody>().mass);
	}

	public void setSpringConstant() {

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
		tempForce.y = Mathf.Lerp(theForce.relativeForce.y, supportForce + (90 * stiffness), currentTime);
		theForce.relativeForce = tempForce;

		previousStiffness = stiffness;

	}
}
