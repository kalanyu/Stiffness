using UnityEngine;
using System.Collections;

public class ExperimentController : MonoBehaviour {

  public GameObject cylinder;
  private SpringJoint slingJoint;
  private float currentTime = 0.0f; 
	private bool lerping = false;
	private bool lowHigh = false;
    // Use this for initialization
  void Start () {
    cylinder = GameObject.Find("Weight");
    slingJoint = cylinder.GetComponent<SpringJoint>();
  }

	// Update is called once per frame
	void Update () {

		if(Input.GetKey(KeyCode.H)) {
			lowHigh = true;
			lerping = true;
	    }

	    if (Input.GetKey(KeyCode.L))
	    {
			lowHigh = false;
			lerping = true;
	    }

		if (Input.GetKey(KeyCode.D)) {
			Destroy(GameObject.Find("Ceiling"));
			Instantiate(Resources.Load("Ceiling"));
			cylinder = GameObject.Find("Weight");
			slingJoint = cylinder.GetComponent<SpringJoint>();
		}

		if (lerping)
		{
			currentTime += 1.0f/60.0f;
			if(lowHigh) 
			{
				slingJoint.spring = Mathf.Lerp(5.0f, 40.0f, currentTime);
			} else {
				slingJoint.spring = Mathf.Lerp(40.0f, 5.0f, currentTime);
			}
		}

		if (currentTime >= 1) {
			lerping = false;
			currentTime = 0;
		}


	}
}
