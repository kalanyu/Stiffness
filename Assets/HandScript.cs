using UnityEngine;
using System.Collections;

public class HandScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision collision) {
		this.transform.parent = collision.gameObject.transform;
//		this.GetComponent<Rigidbody>().isKinematic = true;
//		Debug.Log (collision.gameObject.name);

	}

	void OnTriggerEnter(Collider other) {
		Debug.Log (other.name);
		var weightPos = other.transform.position;
		weightPos.y -= 0.128f;
		this.transform.position = weightPos;
		this.transform.parent = other.gameObject.transform;

    }
}
