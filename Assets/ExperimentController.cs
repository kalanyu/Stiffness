using UnityEngine;
using System;
using System.IO;
using System.Collections;
using UnityEngine.UI;

public class ExperimentController : MonoBehaviour {

  public GameObject cylinder;
	public GameObject ceiling;
	public AudioSource startingBeeps;
  private SpringJoint slingJoint;
 	private float currentTime = 0.0f; 
	private bool lerping = false;
	private bool lowHigh = false;
	public Canvas choiceSelector; 

	private float trialLimit = 400.0f;
	private float trialProgress = 0.0f;
	private int currentTrial;
	private int currentIteration;
	private bool inExperiment = false;

	private TCPClientManager tcpClient;
	private string directoryPath;
	private FileManager fileManager;
	private FileManager expParamReader;
	private string[] expParamerters;
	private string[] currentTrialParameters;

    // Use this for initialization
  void Start () {
		directoryPath = Path.GetFullPath(".");
		tcpClient = GameObject.Find("TCPClientManager").GetComponent<TCPClientManager>();
		if(tcpClient == null) {
			Debug.Log("null");
		}
		choiceSelector.enabled = false;
		StartExperiment();
		 StartCoroutine(StartTrial());

  }

	//initialize everything
	void StartExperiment () {
		//experiment parameters (weight one, weight two
		expParamReader = new FileManager(directoryPath, "/ExperimentParameters/lowlow");
		expParamerters = expParamReader.readAllLinesFromFiles();
		expParamReader.closeFile();

		var fileName = DateTime.Now.ToString("dd_MMMM_yyyy_hh_mm");
		fileManager = new FileManager(directoryPath, "/ExperimentResults/" + fileName + ".csv");
		currentTrial = 0;
		//one trial has two interations
		currentIteration = 1;

		tcpClient.connect();
	}

	IEnumerator StartTrial () {
		//add waiting time
		startingBeeps.Play();
		yield return new WaitForSeconds(4f);
		//play sounds to notice that the trial is starting

		if (currentIteration == 1) {
			currentTrialParameters = expParamerters[currentTrial].Split(','[0]);
			currentTrial += 1;
		}

		if (ceiling == null) {
			ceiling = Instantiate(Resources.Load("Ceiling")) as GameObject;
			ceiling.name = "Ceiling";
			foreach(Rigidbody body in ceiling.GetComponentsInChildren<Rigidbody>()) {
				if (body.name == "Weight") {
					body.mass = float.Parse(currentTrialParameters[currentIteration-1]);
				}
			}
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
		if(currentIteration == 2) {
			choiceSelector.enabled = true;
			currentIteration = 1;
		} else {
			currentIteration += 1;
			StartCoroutine(StartTrial());
		}
	}

	void StopExperiment () {
		fileManager.closeFile();

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
		
//		if (Input.GetKey(KeyCode.D)) {
//			if(!inExperiment) {
//				 StartCoroutine(StartTrial());
//			
//			}
//		}
		
//		Debug.Log(tcpClient.serverSignals[0]);

		if (tcpClient.serverSignals[0] > 0.5)
		{
			var tmpStiff = tcpClient.serverSignals[0];
			currentTime += 1.0f/60.0f;
//			if(lowHigh) 
//			{
			//how about min-maxing and use the value as time increment?
				//should be normalizing here
				slingJoint.spring = Mathf.Lerp(5.0f, 5.0f + (40.0f * ((tmpStiff - 0.5f) * 2f)), currentTime);
//			} else {
//				slingJoint.spring = 5.0f;
//			}
		} else {

//		if (currentTime >= 1) {
			lerping = false;
			currentTime = 0;
		}

	}

	public void ChoiceSelected(int choiceIndex) {
//		Debug.Log(choiceIndex);
		//writes answer to file
		choiceSelector.enabled = false;
		fileManager.writeFileWithMessage(choiceIndex + "\n\r");
		
		if (currentTrial == expParamerters.Length) {
			StopExperiment();
		} else {
			StartCoroutine(StartTrial());
		}
	}
	
	void onApplicationQuit() {
		//make sure everything is closed
		StopTrial();
		StopExperiment();
	}
}
