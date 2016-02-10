using UnityEngine;
using System.Collections;

public class WeightCylinder : MonoBehaviour {

	private bool printed = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (gameObject.GetComponent<Rigidbody> ().IsSleeping() && !printed) {
			Debug.Log(gameObject.transform.position.y);
			printed = true;
		}
	}
}
