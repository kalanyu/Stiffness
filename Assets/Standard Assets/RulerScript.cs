using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RulerScript : MonoBehaviour {
	public float length;
	public float lengthCM;
	private Transform rulerTransform;
	private List<GameObject> lengthTexts;
	// Use this for initialization
	void Start () {
		rulerTransform = GameObject.Find("Ruler").transform;
		lengthTexts = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.LeftBracket) && length > 0) {
			length -= 1; //point upwards
		}

		if (Input.GetKeyDown(KeyCode.RightBracket)) {
			length += 1; //point downwards
		}

		Debug.DrawRay(this.transform.position, this.transform.parent.rotation * (Vector3.down * (lengthCM *length)) , Color.yellow);

		while (lengthTexts.Count > (int)length+1) {
			var obj = lengthTexts[lengthTexts.Count - 1];
			Destroy(obj);
			lengthTexts.Remove(obj);
		}


		for (int i = lengthTexts.Count; i < (int)length+1; i++ ) {
				var newText = LengthText.CreateLengthText("   " + i.ToString() + " cm");
				var rt = rulerTransform.position;
				newText.transform.position = (new Vector3(rt.x, rt.y - (i * lengthCM), rt.z));
				newText.transform.parent = rulerTransform;
				lengthTexts.Add(newText);
		}

		foreach (var item in lengthTexts) {
			var startLoc = item.transform.position;
			Debug.DrawRay (startLoc,this.transform.parent.rotation *(Vector3.left * (lengthCM * 0.5f)), Color.yellow);
			Debug.DrawRay (startLoc,this.transform.parent.rotation *(Vector3.right * (lengthCM * 0.5f)), Color.yellow);

		}
	}

}
