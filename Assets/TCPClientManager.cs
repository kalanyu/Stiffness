using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Collections.Generic;

public class TCPClientManager : MonoBehaviour {
	
	private bool mRunning;
    private String[] values;
	public String msg = "";
    Thread rThread;
	TcpClient client;
//	private List<List<float> > signals = new List<List<float>>();
	public float[] serverSignals = {0.0f,0.0f,0.0f,0.0f,0.0f,0.0f};
	public int bufferSize = 10;
	public int channel = 6;

	public delegate void ConnectionStatusChanged(String status);
	public ConnectionStatusChanged statusChanged;

	void Start()
    {
		if (statusChanged != null) {
			statusChanged("Creating Connection");
		}
    }

	public void connect()
	{

		try {
			
			client = new TcpClient("192.168.100.155",6353);
			if (client.Connected) {
				mRunning = true;
				ThreadStart ts2 = new ThreadStart(Reads);
				rThread = new Thread(ts2);
				rThread.Start();
	
				if (statusChanged != null) {
					statusChanged("Connected");
				}
			}
		} catch {
			Debug.Log("Cannot reach server, reestablishing connection in 5 seconds");
			if (statusChanged != null) {
				statusChanged("Connection failed, reconnecting..");
			}
			StartCoroutine(reconnect());
		}
	}

	public IEnumerator reconnect() {
		yield return new WaitForSeconds(5 * Time.timeScale);
		connect();
	}
	
    public void stopListening()
    {
        mRunning = false;
		try {
			if (client.Connected) {
				client.Close();
			}
			if (statusChanged != null) {
				statusChanged("Connection closed");
			}
		} catch {
			//not connecting to any server
		}
	}
	
	void Reads()
	{
		
		while(mRunning)
		{
			NetworkStream stream = client.GetStream();
				
//				Debug.Log("Client fetched, checking data availability");
				if (stream.DataAvailable)
				{
//					Debug.Log("Prepare to read");
					StreamReader reader = new StreamReader(stream);
					msg = reader.ReadLine();

					if(!msg.StartsWith("Welcome") && msg.Contains(","))
					{
						values = msg.Split(","[0]);
					
						for (int j = 0; j < channel; j++)
						{	
							serverSignals[j] = float.Parse(values[j]);
//							signals[j].Add(float.Parse(values[j]));
//							if (signals[j].Count > bufferSize)
//							{
//								serverSignals[j] = 0.0f;
//								float temp = 0.0f;
//								for (int k = 0; k < bufferSize; k ++)
//								{
//									temp = temp + signals[j][k];
//								}
//								temp = temp/bufferSize;
//								serverSignals[j] = temp;
//								signals[j].Clear();	
//								
//							}	
						}

//						Debug.Log(serverSignals);
					}
//					Debug.Log(msg);					
					
					StreamWriter writer = new StreamWriter(stream);
					writer.WriteLine("Ack\n\r");
					writer.Flush();
//					writer.AutoFlush = true;
//					Debug.Log("WriteBack");				
				}
			}
	}
	
	  void OnApplicationQuit()
    {
        // stop listening thread
        stopListening();
        // wait fpr listening thread to terminate (max. 500ms)
        rThread.Join(500);
    }
}