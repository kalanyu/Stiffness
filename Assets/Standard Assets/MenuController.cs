using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System;
using UnityEngine.UI;
using UnityEngine;


public class MenuController : MonoBehaviour {
	public GameObject nameField;
	public GameObject surnameField;
	public GameObject bodyTypeField;
	public GameObject ageField;
	public GameObject sexField;
	public GameObject errorMessage;
	private List<string> errorMsg;
	public Canvas entryMenu;
	// Use this for initialization
	void Start () {
		errorMsg = new List<string>();
		
	}
	
	void Awake () {

	}
	// Update is called once per frame
	void Update () {
			
	}

	Participant CheckParticipantInfo () {
		Regex checkSpaces = new Regex(@"\s+");
		Regex checkCharacters = new Regex(@"[\'\""\<\>\%\;\(\)\&\+\-\_\&\@]+");
		Regex checkNumbers = new Regex(@"[\0-9]+");
		
		Text name = nameField.transform.Find("Text").GetComponent<Text>();
		Text surname = surnameField.transform.Find("Text").GetComponent<Text>();
		Text bodyTypeText = bodyTypeField.transform.Find("Text").GetComponent<Text>();
		Text ageText = ageField.transform.Find("Text").GetComponent<Text>();
		Text sexText = sexField.transform.Find("Text").GetComponent<Text>();
		
		string date = DateTime.Now.ToString("MMddyyyy");
		//set participant's information
		Participant player = GameObject.Find("PlayerObject").GetComponent<Participant>();
		player.playDate = date;
		
		errorMsg.Clear();
		
		if(checkCharacters.IsMatch(name.text) || checkSpaces.IsMatch(name.text) || checkNumbers.IsMatch(name.text) || name.text.Length == 0)
		{
			errorMsg.Add("Name");
		} else {
			player.name = name.text;
		}
		
		if(checkCharacters.IsMatch(surname.text) || checkSpaces.IsMatch(surname.text) || checkNumbers.IsMatch(surname.text) || surname.text.Length == 0)
		{
			errorMsg.Add("Surname");
		} else {
			player.surname = surname.text;
		}
		
		int education;
		if (!int.TryParse(bodyTypeText.text, out education) || (education <= 0 || education > 3))
		{
			errorMsg.Add("BodyType");
		} else {
			player.bodyType = education;
		}
		
		int sex; 
		if (!int.TryParse(sexText.text, out sex) || (sex <= 0 || sex > 2))
		{
			errorMsg.Add("Sex");
		} else {
			player.sex = sex;
		}
		
		int age;
		if (!int.TryParse(ageText.text, out age) || (age <= 0))
		{
			errorMsg.Add("Age");
		} else {
			player.age = age;
		}
		
		
		if(errorMsg.Count > 0)
			return null;
		else
		{
			
			DontDestroyOnLoad(player);
			return player;	
		}
	}
	
	public void InitiateExperimentPattern(int patternNumber)
	{
		var	directoryPath = Path.GetFullPath(".");

		Directory.CreateDirectory(directoryPath + "/ExperimentResults");
		var participantDirectories = new DirectoryInfo (directoryPath + "/ExperimentResults/");
		var participants = participantDirectories.GetDirectories ();
		Array.Sort(participants, (dir1, dir2) => dir1.Name.CompareTo (dir2.Name));
		

		var recentParticipantsNumber = 0;
		if (participants.Count() > 0) {
			recentParticipantsNumber = int.Parse(participants.Last().Name.Substring(1));

		}

		var newParticipantNumber = String.Format("S{0:D4}", recentParticipantsNumber + 1);

		var resultDirectory = directoryPath + "/ExperimentResults/" + newParticipantNumber + "/";
		Directory.CreateDirectory(resultDirectory);		
		

		if(this.CheckParticipantInfo())
		{		
			Participant player = GameObject.Find("PlayerObject").GetComponent<Participant>();
			player.pattern = patternNumber;
			player.directory = resultDirectory;

			if(File.Exists(player.directory + "/" + "info_pattern_" + patternNumber.ToString() + ".txt"))
			{
				File.Delete(player.directory + "/" + "info_pattern_" + patternNumber.ToString() + ".txt");	
			}
			
            using (StreamWriter sw = File.CreateText(player.directory + "/" + "info_pattern_" + patternNumber.ToString() + ".txt"))
            {
                sw.WriteLine(player.playDate);
                sw.WriteLine("Name:" + player.name);
                sw.WriteLine("Surname:" + player.surname);
				sw.WriteLine("Age:" + player.age);
				switch(player.bodyType)
				{
					case 1:
						sw.WriteLine("BodyType:Slim");
						break;
					case 2:
						sw.WriteLine("BodyType:Normal");
						break;
					case 3:
						sw.WriteLine("BodyType:Muscular");
						break;
					case 4:
						sw.WriteLine("BodyType:Overweight");
						break;
				}
				if(player.sex == 1)
					sw.WriteLine("Sex: Male");
				else
					sw.WriteLine("Sex: Female");

				var tcpClient = GameObject.Find("TCPClientManager");
				if (tcpClient != null) {
					var clientComponent = tcpClient.GetComponent<TCPClientManager>();
					sw.WriteLine("SamplingRate:" + clientComponent.samplingRate);
					sw.WriteLine("FilteredFileName:" + clientComponent.filteredFileName);
					sw.WriteLine("RawFileName:" + clientComponent.rawFileName);
				} else {
					Debug.Log("not connected to the data server");
				}
            }
	        
			
			switch (patternNumber)
			{
				case 1:
					GameObject.Find("ExperimentController").GetComponent<ExperimentController>().ExperimentModeSelected(1);
					break;
				case 2:
					GameObject.Find("ExperimentController").GetComponent<ExperimentController>().ExperimentModeSelected(2);
					break;
				default:
					break;
			}
		}
		else
		{
			Text errorText = errorMessage.GetComponent<Text>();
			string output = "Error(s) detected: " + string.Join(", ", errorMsg.ToArray());
			errorText.text = output;
		}
	}
}
