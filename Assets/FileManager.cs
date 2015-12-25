using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class FileManager {
	StreamWriter fileWriter;
	StreamReader fileReader;
	string filePath;

	public FileManager (string directoryPath, string fileName, char mode)
	{
		switch (mode) {
		case 'r':
			try {
				fileReader = new StreamReader(directoryPath+fileName);
			}
			catch (Exception err)
			{
				Debug.Log (err.Message);
			}
			break;
		case 'w':
			try {
				Directory.CreateDirectory(directoryPath + "/ExperimentResults");
				filePath = directoryPath + fileName;
				fileWriter = new StreamWriter(directoryPath + fileName);	
			}
			catch (Exception err)
			{
				Debug.Log(err.Message);
			}
			break;
		default:
			break;
		}

	}
	
	public FileManager (string directoryPath, string fileName)
	{
		try {
			Directory.CreateDirectory(directoryPath + "/ExperimentResults");
			filePath = directoryPath + fileName;
			fileWriter = new StreamWriter(filePath);	
		}
		catch (Exception err)
		{
			Debug.Log(err.Message);
		}
	}

	public void writeFileWithMessage(string text)
	{
		if ( fileWriter != null )
		{
			fileWriter.WriteLine( text );
			fileWriter.Flush();
			Debug.Log("written");
		}
		else
		{
			Debug.Log("fileWriter failed to write the message");
		}	
	}

	public string[] readAllLinesFromFiles(){
		List<String> lines = new List<String>();

		if (fileReader != null)
		{

			while (!fileReader.EndOfStream)
			{
				string line = fileReader.ReadLine();
				lines.Add(line);
			}
		}
		return lines.ToArray();
	}

	public void closeFile()
	{
		if ( fileWriter != null )
		{
			fileWriter.Close();
		}
		else
		{
			Debug.Log("fileWriter failed to close the file");
		}
	}

	
	
}