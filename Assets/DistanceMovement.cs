using UnityEngine;
using System;
using System.Collections;

public class DistanceMovement : MonoBehaviour {
    public float stiffness = 1.0f;
    public float weightMass = 0.0f;
	private float initPos = 0.911f;
    
	// Use this for initialization
	void Start () {
	    weightMass = GameObject.Find("Weight").GetComponent<Rigidbody>().mass;
		StartCoroutine(SpawnCube(4));
	}
	
	// Update is called once per frame
	void Update () {
	       
        if (Input.GetKey(KeyCode.UpArrow)) {
            var tempStiffness = stiffness + 0.1f;
            stiffness = Math.Min(1, tempStiffness);
        }

        if (Input.GetKey(KeyCode.DownArrow)) {
            var tempStiffness = stiffness - 0.1f;
            stiffness = Math.Max(0.1f, tempStiffness);
        }
        
		var newPosition = 0.911f - ((weightMass * 9.81f / (1000 * stiffness)));

		Debug.Log(stiffness);
		var newPosV = this.transform.position;
		newPosV.y = newPosition;
		this.transform.position = newPosV;

	}

	  void OnCollisionEnter(Collision collision) {
	    weightMass = GameObject.Find("Weight").GetComponent<Rigidbody>().mass;
	  }

	IEnumerator SpawnCube(float seconds) {
		Debug.Log("yield");
		yield return new WaitForSeconds(seconds);
		GameObject.Find("Weight").GetComponent<Rigidbody>().useGravity = true;

	}
}
