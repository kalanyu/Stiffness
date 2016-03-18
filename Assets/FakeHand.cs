using UnityEngine;
using System.Collections;
using System;

public class FakeHand : MonoBehaviour {
	public float springConstant;
	public float stiffness = 0.0f;
	private float lengthPerCM = 0.0241f;
	private float currentTime = 0.0f;
	private float weightValue;
	private Vector3 originalPosition;
	private Rigidbody rigidBody;
	// Use this for initialization
	void Start () {
		weightValue = GameObject.Find("Cube").GetComponent<Rigidbody>().mass;
		rigidBody = this.GetComponent<Rigidbody>();
		originalPosition = rigidBody.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.UpArrow)) {
			var tempStiffness = stiffness + 0.1f;
			stiffness = Math.Min(1, tempStiffness);
			currentTime = 0;
		}

		if (Input.GetKey(KeyCode.DownArrow)) {
			var tempStiffness = stiffness - 0.1f;
			stiffness = Math.Max(0, tempStiffness);
			currentTime = 0;
		}
//		currentTime = Math.Max(1, (currentTime + 1)/1.0f);
		var newPos = rigidBody.position;
		newPos.y = originalPosition.y + ( ( (weightValue*10) / springConstant * lengthPerCM) * stiffness );
		rigidBody.MovePosition(newPos);
		
	}
}
