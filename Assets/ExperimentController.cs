using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
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

	private float trialLimit = 4.0f;
	private float trialProgress = 0.0f;
	private int currentTrial;
	private int currentIteration;
	private bool inExperiment = false;

	private TCPClientManager tcpClient;
	private string directoryPath;
	private FileManager fileManager;
	private FileManager expParamReader;
	private FileManager filteredSignalsRecorder;
	private FileManager rawSignalsRecorder;
	private string[] expParamerters;
	private string[] currentTrialParameters;
	public Text networkConnectionStatus;
	private float stiffness;
	public float maxStrengthRatio = 1.0f;
	private float stiffnessThreshold = 1.5f;
	private string resultDirectory;
	    // Use this for initialization
	 void Start () {
			directoryPath = Path.GetFullPath(".");

			Directory.CreateDirectory(directoryPath + "/ExperimentResults");
			var participantDirectories = new DirectoryInfo (directoryPath + "/ExperimentResults/");
			var participants = participantDirectories.GetDirectories ();
			Array.Sort(participants, (dir1, dir2) => dir1.Name.CompareTo (dir2.Name));
			

			var recentParticipantsNumber = 0;
			if (participants.Count() > 0) {
				recentParticipantsNumber = int.Parse(participants.Last().Name.Substring(1));

			}

			var newParticipantNumber = String.Format("S{0:D4}", recentParticipantsNumber + 1);

			resultDirectory = directoryPath + "/ExperimentResults/" + newParticipantNumber + "/";
			Directory.CreateDirectory(resultDirectory);			
			

			tcpClient = GameObject.Find("TCPClientManager").GetComponent<TCPClientManager>();
			stiffnessBar.enabled = false;
	
			if(tcpClient != null) {
				tcpClient.statusChanged += UpdateNetworkStatus;
				tcpClient.connect();
				tcpClient.IncomingDataFromSensor += IncomingDataFromSensor;
			}
	//			slingJoint = GameObject.Find("Cube").GetComponentInChildren<SpringJoint>();
	
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
				expName = "highlow";
				break;
			default:
				break;
		}
		//experiment parameters (weight one, weight two
		try {
			expParamReader = new FileManager(directoryPath, "/ExperimentParameters/" + expName + ".csv",'r');
			expParamerters = expParamReader.readAllLinesFromFiles();
	
			var fileName = DateTime.Now.ToString("dd_MMMM_yyyy_hh_mm") + "_" + expName;
			fileManager = new FileManager(resultDirectory,fileName + ".csv");
			
			if (tcpClient.connecting) {
				filteredSignalsRecorder = new FileManager(resultDirectory,fileName + "_filtered.csv");
				rawSignalsRecorder = new FileManager(resultDirectory,fileName + "_raw.csv");
			}
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
		yield return new WaitForSeconds(4f * Time.timeScale);
		//play sounds to notice that the trial is starting
		inExperiment = true;

		if (currentIteration == 1) {
			currentTrialParameters = expParamerters[currentTrial].Split(',');
			currentTrial += 1;
		}
		if (ceiling == null) {
			ceiling = Instantiate(Resources.Load("WeakSpring")) as GameObject;
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
		//loads trial files

//		Debug.Log("Start Trial");
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
		filteredSignalsRecorder.closeFile();
		rawSignalsRecorder.closeFile();
		Application.Quit();
	}
	// Update is called once per frame, controls joint stiffness here
	void Update () {

		if (inExperiment) {
			trialProgress += Time.deltaTime;

			if (trialProgress > trialLimit * Time.timeScale) {
				StopTrial();
			}
		}
		
//		if (Input.GetKey(KeyCode.D)) {
//			if(!inExperiment) {
//				 StartCoroutine(StartTrial());
//			
//			}
//		}
		
		if (stiffnessBar.enabled) {
			var tmpLocalScale = stiffnessGuage.localScale;
			stiffnessGuage.localScale = new Vector3(tmpLocalScale.x, stiffness, tmpLocalScale.z);
		}

		if (slingJoint != null) {
			if (stiffness > 0.2f)
			{
				currentTime += 1.0f/60.0f;
				slingJoint.spring = Mathf.Lerp(40.7f, 40.7f + (121.85f * stiffness), currentTime);
				slingJoint.damper = 15;
			} else {
				slingJoint.spring = 40.7f;
				slingJoint.damper = 7;
				currentTime = 0;
			}
		}
	}

	public void ChoiceSelected(int choiceIndex) {
		//writes answer to file
		choiceSelector.enabled = false;
		fileManager.writeFileWithMessage(currentTrial + "," + choiceIndex + "\n");
		
		if (currentTrial == expParamerters.Length) {
			StopExperiment();
		} else {
			StartCoroutine(StartTrial());
		}
	}
	
	void onApplicationQuit() {
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
		return value / stiffnessThreshold;
	}

	public void ExperimentModeSelected(int mode) {
		StartExperiment(mode);
		expmodeMenu.enabled = false;
	}

	void IncomingDataFromSensor(float[] data) {
		float flexor = data[0] * (1/maxStrengthRatio);
		float extensor = data[1] * (1/maxStrengthRatio);
		
		if (Mathf.Abs(flexor + extensor) < 0.25) {
			stiffness = Mathf.Min(1, minmaxNormalize(flexor + extensor));
		} else {
			stiffness = 0;
		}

		if(inExperiment && tcpClient.connecting) {
			//should write signals to file here
			filteredSignalsRecorder.writeFileWithMessage(currentTrial + "," + DateTime.Now.ToString("mm:ss:ffff") + "," + data[0] + "," + data[1]);
			rawSignalsRecorder.writeFileWithMessage(currentTrial + "," + DateTime.Now.ToString("mm:ss:ffff") + "," + data[2] + "," + data[3]);
		}
	}
}
