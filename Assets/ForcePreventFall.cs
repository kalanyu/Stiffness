using UnityEngine;
using System.Collections;
using System;

public class ForcePreventFall : MonoBehaviour {
	private ConstantForce theForce;
	private float resistForce;
	public float stiffness = 1.0f;
	private float currentTime = 0.0f;
	// Use this for initialization
	void Start () {
		resistForce = this.GetComponent<ConstantForce>().relativeForce.y;
		theForce = this.GetComponent<ConstantForce>();
	}
	
	// Update is called once per frame
	void Update () {
			if (Input.GetKey(KeyCode.UpArrow)) {
			var tempStiffness = stiffness + 0.05f;
			stiffness = Math.Min(1, tempStiffness);
			currentTime = 0;
		}

		if (Input.GetKey(KeyCode.DownArrow)) {
			var tempStiffness = stiffness - 0.05f;
			stiffness = Math.Max(0, tempStiffness);
			currentTime = 0;
		}
	
		currentTime = Math.Max(1, (currentTime + 1)/60.0f);
		var tempForce = theForce.relativeForce;
		tempForce.y = Mathf.Lerp(theForce.relativeForce.y, resistForce + (100 * stiffness), currentTime);
		theForce.relativeForce = tempForce;

	}
}
