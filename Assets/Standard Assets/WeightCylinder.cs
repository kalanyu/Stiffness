using UnityEngine;
using System.Collections;

public class WeightCylinder : MonoBehaviour {

	private bool printed = false;
	public bool collided = false;
	// Use this for initialization
	void Start () {
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.name == "Cube") {
			collided = true;
		}

	}	// Update is called once per frame

	void Update () {
		if (gameObject.GetComponent<Rigidbody> ().IsSleeping() && !printed) {
			Debug.Log(gameObject.transform.position.y);
			printed = true;
		}
	}
}
