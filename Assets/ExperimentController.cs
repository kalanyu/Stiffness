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

	public Canvas choiceSelector; 
	public Canvas stiffnessBar;
	public Canvas expmodeMenu;
	public RectTransform stiffnessGuage;

	private float trialLimit = 2.0f;
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
	public Text networkConnectionStatus;
	
    // Use this for initialization
  void Start () {
		directoryPath = Path.GetFullPath(".");
		tcpClient = GameObject.Find("TCPClientManager").GetComponent<TCPClientManager>();
		stiffnessBar.enabled = false;

		if(tcpClient != null) {
			tcpClient.statusChanged += UpdateNetworkStatus;
			tcpClient.connect();
		}

		choiceSelector.enabled = false;
  }

	//initialize everything
	void StartExperiment (int type) {
		startingBeeps.Play();
		string expName = "";
		
		switch(type) {
			case 1:
				expName = "lowlow";
				break;
			case 2:
				expName = "highhigh";
				break;
			case 3:
				expName = "lowhigh";
				break;
			case 4:
				expName = "highhigh";
				break;
			default:
				break;
		}
		//experiment parameters (weight one, weight two
		try {
			expParamReader = new FileManager(directoryPath, "/ExperimentParameters/" + expName + ".csv",'r');
			expParamerters = expParamReader.readAllLinesFromFiles();
			expParamReader.closeFile();
	
			var fileName = DateTime.Now.ToString("dd_MMMM_yyyy_hh_mm");
			fileManager = new FileManager(directoryPath, "/ExperimentResults/" + fileName + ".csv");
			currentTrial = 0;
			//one trial has two interations
			currentIteration = 1;
	
			StartCoroutine(StartTrial());
		} catch {
			Debug.Log("sth wrong with file");
		}
	}

	IEnumerator StartTrial () {
		//add waiting time
		startingBeeps.Play();
		yield return new WaitForSeconds(4f);
		//play sounds to notice that the trial is starting

		if (currentIteration == 1) {
			currentTrialParameters = expParamerters[currentTrial].Split(',');
			currentTrial += 1;
		}
		if (ceiling == null) {
			ceiling = Instantiate(Resources.Load("Ceiling")) as GameObject;
			ceiling.name = "Ceiling";
			foreach(Rigidbody body in ceiling.GetComponentsInChildren<Rigidbody>()) {
				if (body.name == "Weight") {
					body.mass = float.Parse(currentTrialParameters[currentIteration]);
					Debug.Log(expParamerters.Length + "," + currentTrial + "," + currentIteration + "," + body.mass );

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
			currentIteration = 2;
			StartCoroutine(StartTrial());
		}
	}

	void StopExperiment () {
		fileManager.closeFile();
		Application.Quit();
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
		
		Debug.Log(tcpClient.serverSignals[2]);
		if (stiffnessBar.enabled) {
			var tmpLocalScale = stiffnessGuage.localScale;
			stiffnessGuage.localScale = new Vector3(tmpLocalScale.x, minmaxNormalize(tcpClient.serverSignals[0]), tmpLocalScale.z);
		}

		if (slingJoint != null) {
			slingJoint.spring = 15.0f;
//			if (minmaxNormalize(tcpClient.serverSignals[2]) > 0.5)
//			{
//				var tmpStiff = minmaxNormalize(tcpClient.serverSignals[0]);
//				currentTime += 1.0f/60.0f;
//				slingJoint.spring = Mathf.Lerp(5.0f, 5.0f + (10.0f * tmpStiff), currentTime);
//				Debug.Log(slingJoint.spring);
//			} else {
//				slingJoint.spring = 5.0f;
//				currentTime = 0;
//			}
		}
	}

	public void ChoiceSelected(int choiceIndex) {
//		Debug.Log(choiceIndex);
		//writes answer to file
		choiceSelector.enabled = false;
		fileManager.writeFileWithMessage(choiceIndex + "\n");
		
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

	void UpdateNetworkStatus(String status) {
		networkConnectionStatus.text = status;
		if (status == "Connected") {
			stiffnessBar.enabled = true;
		}
	}

	private float minmaxNormalize(float value) {
//		var normed = ((value - 0.5f) / 0.5f);
		var normed = ((value - 0.5f) / 0.05f);
		return normed < 0 ? 0 : normed;
	}

	public void ExperimentModeSelected(int mode) {
		StartExperiment(mode);
		expmodeMenu.enabled = false;
	}
}
