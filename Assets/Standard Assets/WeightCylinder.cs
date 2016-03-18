using UnityEngine;
using System;
using System.Collections;

public class WeightCylinder : MonoBehaviour {

	private bool printed = false;
	private SpringJoint slingJoint;
	public float stiffness = 1.0f;
	private float previousStiffness = 0.0f;
	private float currentTime = 0.0f;
	public bool collided = false;

	// Use this for initialization
	void Start () {
		slingJoint = this.GetComponent<SpringJoint>();
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
		
		Debug.Log(slingJoint.spring);
		currentTime = Math.Max(1, (currentTime + 1)/60.0f);

		if (stiffness > 0.1f)
		{
			if (slingJoint.gameObject.GetComponent<Rigidbody>().IsSleeping()) {
				slingJoint.gameObject.GetComponent<Rigidbody>().WakeUp();
			}
			slingJoint.spring = Mathf.Lerp(40.7f, 40.7f + (121.85f * stiffness), currentTime);
			slingJoint.damper = 15;
		} else {
			slingJoint.spring = 40.7f;
			slingJoint.damper = 7;
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
