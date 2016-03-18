using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEngine.UI;


public class ExperimentController : MonoBehaviour {
	public int experimentType = 0;
	private string[] stimulusName = { "Spring", "ForcePickupHand", "StiffnessPickupHand", "ForcePreventFall", "StiffnessPreventFall"};
	//experiment type: 0 stiffness pull up, 1 force pickup, 2 stiffness pickup, 3 force prevent fall, 4 stiffness prevent fall
	// Divides into two patterns , loop between each experiment for 90 trials and pause with confirm button
	public GameObject system;
	public AudioSource startingBeeps;
	private object handForce;


	public Canvas choiceSelector; 
	public Canvas stiffnessBar;
	public Canvas stiffnessBar2;
	public Canvas expmodeMenu;
	public Canvas continueMenu;
	public Canvas quitMenu;

	public RectTransform stiffnessGuage;
	public RectTransform stiffnessGuage2;

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
	public Text experimentTrialCount;

	public float stiffness;
	private float baseStiffness;

	public float maxStrengthRatio = 1.0f;
	private float stiffnessThreshold = 1.5f;
	private string resultDirectory;
	private int pattern;
	
	private WeightCylinder weightObject;
	private DateTime timeSinceLastCalled;

	List<string> expName = new List<String>();
	List<string> expDescriptions = new List<String>();


	 // Use this for initialization
	 void Start () {
		tcpClient = GameObject.Find("TCPClientManager").GetComponent<TCPClientManager>();

		if(tcpClient != null) {
			tcpClient.statusChanged += UpdateNetworkStatus;
			tcpClient.connect();
			tcpClient.IncomingDataFromSensor += IncomingDataFromSensor;
		}


		choiceSelector.enabled = false;
		continueMenu.enabled = false;
		quitMenu.enabled = false;
	}


	IEnumerator StartTrial () {
		//add waiting time
		startingBeeps.Play();
		yield return new WaitForSeconds(4f * Time.timeScale);
		//play sounds to notice that the trial is starting
		inExperiment = true;

		if (currentIteration == 1) {
			currentTrialParameters = expParamerters[currentTrial].Split(',');
			experimentTrialCount.text = (currentTrial+1) + "/" + expParamerters.Length + " : " + expName.Count + " exp remaining";

			currentTrial += 1;
			if (expName[0].StartsWith("low")) {
				maxStrengthRatio = 2;
			} else {
				maxStrengthRatio = 1;
			}
		}
		else {
			if (expName[0].EndsWith("low")) {
				maxStrengthRatio = 2;
			} else {
				maxStrengthRatio = 1;
			}
		}
		
//		Debug.Log(expName[0] + ":iteration " + currentIteration + ": musclePowerNeeded " + (1/maxStrengthRatio));

		if (system == null) {
			system = Instantiate(Resources.Load(stimulusName[experimentType])) as GameObject;
			system.name = "system";

			switch(experimentType) {
				case 0:
					handForce = system.GetComponentInChildren<WeightCylinder>();
					break;
				case 1:
					handForce = system.GetComponentInChildren<ForcePickup>();
					break;
				default:
					break;
			}

			foreach(Rigidbody body in system.GetComponentsInChildren<Rigidbody>()) {
				if (body.name == "Weight") {
					body.mass = float.Parse(currentTrialParameters[currentIteration]);
					if (handForce is ForcePickup) {
						(handForce as ForcePickup).resetSupportForce();
					}
					
					Debug.Log(expParamerters.Length + "," + currentTrial + "," + currentIteration + "," + body.mass );
				}
			}
		}

		trialProgress = 0.0f;
		//loads trial files
	}

	void StopTrial () {
		for (int i = 0; i < system.transform.childCount; i++)
		{
			Destroy(system.transform.GetChild(i).gameObject);
		}
		Destroy(system);
		
		system = null;
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
		if (tcpClient.connecting) {
			filteredSignalsRecorder.closeFile();
			rawSignalsRecorder.closeFile();
		}

		expName.RemoveAt(0);

		if (expName.Count > 0) {
			LoadNextDescription();
		} else {
			quitMenu.enabled = true;
		}
	}
	// Update is called once per frame, controls joint stiffness here
	void Update () {

		if (inExperiment) {
			trialProgress += Time.deltaTime;

			if (trialProgress > trialLimit * Time.timeScale) {
				StopTrial();
			}
		}
		
		if (Input.GetKey(KeyCode.UpArrow)) {
			var tempStiffness = stiffness + 0.1f;
			stiffness = Math.Min(1, tempStiffness);
		}

		if (Input.GetKey(KeyCode.DownArrow)) {
			var tempStiffness = stiffness - 0.1f;
			stiffness = Math.Max(0, tempStiffness);
		}
		
		if (stiffnessBar.enabled) {
			var tmpLocalScale = stiffnessGuage.localScale;
			stiffnessGuage.localScale = new Vector3(tmpLocalScale.x, baseStiffness, tmpLocalScale.z);
		}
		if (stiffnessBar2.enabled) {
			var tmpLocalScale2 = stiffnessGuage2.localScale;
			stiffnessGuage2.localScale = new Vector3(tmpLocalScale2.x, stiffness, tmpLocalScale2.z);

		}

		if (handForce != null) {
			switch(experimentType) {
				case 0:
					(handForce as WeightCylinder).stiffness = stiffness;
					break;
				case 1:
					(handForce as ForcePickup).stiffness = stiffness;
					break;
				default:
					break;
			}
		}
	}

	public void ChoiceSelected(int choiceIndex) {
		//writes answer to file
		choiceSelector.enabled = false;
		fileManager.writeFileWithMessage(currentTrial + "," + currentTrialParameters[1] + "," + currentTrialParameters[2] + "," + choiceIndex);
		
		if (currentTrial == expParamerters.Length) {
			StopExperiment();
		} else {
			StartCoroutine(StartTrial());
		}
	}

	private float minmaxNormalize(float value) {
		return value / stiffnessThreshold;
	}


#region Trial Preparation

	void StartExperiment (int type) {
		pattern = type;
		string lowlowDesc = "For each trial, two objects with different weight will be displayed sequentially. Your task is to stiff your arm to to align the object's place holder with the onscreen white bar. The task requires low muscle strength. After two objects disappeared, you will need to choose which of the two is heavier.";
		string highhighDesc = "For each trial, two objects with different weight will be displayed sequentially. Your task is to stiff your arm to align the object's place holder with the onscreen white bar. The task requires high muscle strength. After two objects disappeared, you will need to choose which of the two is heavier.";
		string lowhighDesc = "For each trial, two objects with different weight will be displayed sequentially. Your task is to stiff your arm to align the object's place holder with the onscreen white bar. The task requires low muscle strength for the first object and high muscle strength for the second object.After two objects disappeared, you will need to choose which of the two is heavier.";
		string highlowDesc = "For each trial, two objects with different weight will be displayed sequentially. Your task is to stiff your arm to align the object's place holder with the onscreen white bar. The task requires high muscle strength for the first object and low muscle strength for the second object After two objects disappeared, you will need to choose which of the two is heavier.";

		switch(type) {
			case 1:
//				expName = new string[]{"lowlow", "highhigh", "lowhigh", "highlow"}.ToList();
//				expDescriptions = new string[]{lowlowDesc, highhighDesc, lowhighDesc, highlowDesc}.ToList();
				expName = new string[]{"lowlow", "highhigh", "lowhigh", "highlow"}.ToList();
				expDescriptions = new string[]{lowlowDesc, highhighDesc, lowhighDesc, highlowDesc}.ToList();
				break;
			case 2:
				expName = new string[]{"highhigh", "lowlow", "highlow", "lowhigh"}.ToList();
				expDescriptions = new string[]{highhighDesc, lowlowDesc, highlowDesc, lowhighDesc}.ToList();
				break;
			default:
				break;
		}
		
		LoadNextDescription();
	}

	public void ExperimentModeSelected(int mode) {
		directoryPath = Path.GetFullPath(".");
		var participantDirectories = new DirectoryInfo (directoryPath + "/ExperimentResults/");
		var participants = participantDirectories.GetDirectories ();
		Array.Sort(participants, (dir1, dir2) => dir1.Name.CompareTo (dir2.Name));
		

		var recentParticipantsNumber = 0;
		if (participants.Count() > 0) {
			recentParticipantsNumber = int.Parse(participants.Last().Name.Substring(1));

		}

		var currentParticipant = String.Format("S{0:D4}", recentParticipantsNumber);

		resultDirectory = directoryPath + "/ExperimentResults/" + currentParticipant + "/";
		
		StartExperiment(mode);
		expmodeMenu.enabled = false;
	}

	private void LoadNextDescription() {
		stiffnessBar.enabled = true;
		stiffnessBar2.enabled = true;
		continueMenu.enabled = true;

		GameObject.Find("EXPDescription").GetComponent<Text>().text = expDescriptions[0];
		expDescriptions.RemoveAt(0);
	}

	public void LoadNextTrial() {
		continueMenu.enabled = false;
		stiffnessBar.enabled = false;
		stiffnessBar2.enabled = false;
		LoadTask(expName[0]);
	}

	private void LoadTask(string name) {
		try {
			expParamReader = new FileManager(directoryPath, "/ExperimentParameters/" + name + ".csv",'r');
			expParamerters = expParamReader.readAllLinesFromFiles();
	
			var fileName = name;
			fileManager = new FileManager(resultDirectory,fileName + "_p_" + pattern + ".csv");
			
			if (tcpClient.connecting) {
				filteredSignalsRecorder = new FileManager(resultDirectory,fileName + "_filtered"  + "_p_" + pattern + ".csv");
				rawSignalsRecorder = new FileManager(resultDirectory,fileName + "_raw" + "_p_" + pattern + ".csv");
			}
			currentTrial = 0;
			//one trial has two interations
			currentIteration = 1;

			experimentTrialCount.text =  "0/" + expParamerters.Length + " : " + expName.Count + " exp remaining";

			StartCoroutine(StartTrial());
			timeSinceLastCalled = DateTime.Now;
		} catch {
			Debug.Log("sth wrong with file");
		}
	}
#endregion

#region Network Functions

	void UpdateNetworkStatus(String status) {
		networkConnectionStatus.text = status;
	}

	void IncomingDataFromSensor(float[] data, string timestamp) {
		
		float flexor = Math.Max(0, Math.Min(1,data[0] * maxStrengthRatio));
		float extensor = Math.Max(0, Math.Min(1,data[1] * maxStrengthRatio));

		float baseFlexor = Math.Max(0, Math.Min(1,data[0]));
		float baseExtensor = Math.Max(0, Math.Min(1,data[1]));

		baseStiffness = ((baseFlexor + baseExtensor) - Math.Abs(baseFlexor - baseExtensor)) / 2; // 2 comes from clipped strength (1 + 1)

		stiffness = ((flexor + extensor) - Math.Abs(flexor - extensor)) / 2; // 2 comes from clipped strength (1 + 1)
		
		if(inExperiment && tcpClient.connecting) {
			//should write signals to file here
			bool collided = false;
			if (weightObject != null) {
				collided = weightObject.collided;
			}
			//ensure consistency
			var trialNumber = currentTrial;
			int iteration = currentIteration; 
			var collision = collided? "1" : "0";
			filteredSignalsRecorder.writeFileWithMessage(trialNumber + "," + timestamp + "," + data[0] + "," + data[1] + "," + iteration + "," + collision);
			rawSignalsRecorder.writeFileWithMessage(trialNumber + "," + timestamp + "," + data[2] + "," + data[3] + "," + iteration + "," + collision);
			
		}
	}

#endregion

	void onApplicationQuit() {
		StopTrial();
		StopExperiment();
	}

	public void QuitExperiment() {
		Application.Quit();
	}
}
