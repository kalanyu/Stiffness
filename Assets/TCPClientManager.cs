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
	
	void Start()
    {
		Debug.Log("new client");

    }
	public void connect()
	{
		client = new TcpClient("192.168.2.1",6535);
		if (client.Connected) {
			mRunning = true;
			ThreadStart ts2 = new ThreadStart(Reads);
			rThread = new Thread(ts2);
			rThread.Start();
		}
	}
	
    public void stopListening()
    {
        mRunning = false;
		if (client.Connected) {
			client.Close();
		}
	}
	
	void Reads()
	{
		
		while(mRunning)
		{
			NetworkStream stream = client.GetStream();
				
				Debug.Log("Client fetched, checking data availability");
				if (stream.DataAvailable)
				{
					Debug.Log("Prepare to read");
					StreamReader reader = new StreamReader(stream);
					msg = reader.ReadLine();

					if(!msg.StartsWith("whoareyou"))
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
					//Debug.Log(msg);					
					
					StreamWriter writer = new StreamWriter(stream);
					writer.WriteLine("Ack\n\r");
					writer.Flush();
					Debug.Log("WriteBack");				
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