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
//		Debug.Log (collision.gameObject.name);

	}
}
