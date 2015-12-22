using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ExperimentController : MonoBehaviour {

  public GameObject cylinder;
	public GameObject ceiling;
  private SpringJoint slingJoint;
 	private float currentTime = 0.0f; 
	private bool lerping = false;
	private bool lowHigh = false;
	public Canvas choiceSelector; 

	private float trialLimit = 4.0f;
	private float trialProgress = 0.0f;
	private int currentTrial;
	private bool inExperiment = false;

    // Use this for initialization
  void Start () {
		choiceSelector.enabled = false;
		StartExperiment();
  }

	//initialize everything
	void StartExperiment () {
		currentTrial = 0;
	}

	void StartTrial () {
		//add waiting time
		//play sounds to notice that the trial is starting

		currentTrial += 1;

		if (ceiling == null) {
			ceiling = Instantiate(Resources.Load("Ceiling")) as GameObject;
			ceiling.name = "Ceiling";
			slingJoint = ceiling.GetComponentInChildren<SpringJoint>();
		}

		trialProgress = 0.0f;
		inExperiment = true;
		//loads trial files

		Debug.Log("Start Trial");
	}

	void StopTrial () {
		Destroy(GameObject.Find("Weight"));
		Destroy(GameObject.Find("Ceiling"));
		ceiling = null;
		slingJoint = null;
		//shows UI for choice making here

		inExperiment = false;

		//checks if show UI or need another trial
		if(currentTrial % 2 == 0) {
			choiceSelector.enabled = true;
		} else {
			StartTrial();
		}
	}

	// Update is called once per frame, controls joint stiffness here
	void Update () {

		if (inExperiment) {
			trialProgress += Time.deltaTime;
//			Debug.Log(trialProgress);

			if (trialProgress > trialLimit) {
				StopTrial();
			}
		}
		


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
			if(!inExperiment) {
				StartTrial();
			
			}
		}

		if (lerping)
		{
			currentTime += 1.0f/60.0f;
			if(lowHigh) 
			{
				slingJoint.spring = Mathf.Lerp(5.0f, 40.0f, currentTime);
			} else {
				slingJoint.spring = 5.0f;
			}
		}

		if (currentTime >= 1) {
			lerping = false;
			currentTime = 0;
		}

	}

	public void ChoiceSelected(int choiceIndex) {
//		Debug.Log(choiceIndex);
		//writes answer to file
		choiceSelector.enabled = false;
		StartTrial();
	}
}
