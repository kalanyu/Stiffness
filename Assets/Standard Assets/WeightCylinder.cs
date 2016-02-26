using UnityEngine;
using System.Collections;

public class WeightCylinder : MonoBehaviour {

	private bool printed = false;
	public bool collided = false;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame

	void Update () {
		if (gameObject.GetComponent<Rigidbody> ().IsSleeping() && !printed) {
//			Debug.Log(gameObject.transform.position.y);
			printed = true;
		}
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.name == "Placeholder") {
			collided = true;
			var weightPos = this.transform.position;
			var cubeTransform = transform.parent.transform.Find("Cube").transform.position;
			weightPos.y -= 0.128f;
			weightPos.x = cubeTransform.x;
			weightPos.z = cubeTransform.z;
			transform.parent.transform.Find("Cube").transform.position = weightPos;
			transform.parent.transform.Find("Cube").transform.parent = this.transform;
		}
	}
}
