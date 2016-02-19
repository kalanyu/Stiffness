using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.ComponentModel;
using System.Collections.Generic;

public class TCPClientManager : MonoBehaviour {
	
	private bool mRunning;
	public bool connecting = false;
    private String[] values;
	public String msg = "";
	TcpClient client;
//	private List<List<float> > signals = new List<List<float>>();
	public float[] serverSignals = {0.0f,0.0f,0.0f,0.0f,0.0f,0.0f};
	public int bufferSize = 10;
	public int channel = 2;
	
	public delegate void ConnectionStatusChanged(String status);
	public delegate void IncomingDataEvent(float[] data);
	public ConnectionStatusChanged statusChanged;
	public IncomingDataEvent IncomingDataFromSensor;

	void Start()
    {
		if (statusChanged != null) {
			statusChanged("Creating Connection");
		}
    }

	public void connect()
	{
        try
        {

            client = new TcpClient();
            var result = client.BeginConnect("localhost", 6353, null, null);
            var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));

            if (client.Connected)
            {
                mRunning = true;
				var bw = new BackgroundWorker();
				bw.DoWork += new DoWorkEventHandler(Reads);
				bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(reconnectHandler);
//   if (System.Threading.SynchronizationContext.Current == null) {
//            throw new InvalidOperationException("You are on the wrong thread")
//		        }
				bw.RunWorkerAsync();

                client.EndConnect(result);

                if (statusChanged != null)
                {
                    statusChanged("Connected");
					connecting = true;
                }
            }
            else
            {
                client.EndConnect(result);
                throw new Exception();
            }
        }
        catch
        {
//            Debug.Log("Cannot reach server, reestablishing connection in 5 seconds");
            if (statusChanged != null)
            {
                statusChanged("Connection failed, reconnecting..");
            }
            StartCoroutine(reconnect());
        }
	}

	public void reconnectHandler(object sender, RunWorkerCompletedEventArgs e) {
		Dispatcher.Instance.Invoke(connect);
	}

	public IEnumerator reconnect() {
		yield return new WaitForSeconds(5 * Time.timeScale);
		connect();
	}
	
    public void stopListening()
    {
        mRunning = false;
        try
        {
            if (client.Connected)
            {
                NetworkStream stream = client.GetStream();
                if (stream.DataAvailable)
                {
                    Debug.Log("Disconnected avail");

                    StreamReader reader = new StreamReader(stream);
                    msg = reader.ReadLine();
                    StreamWriter writer = new StreamWriter(stream);
                    writer.WriteLine("Disconnect\r");
                    writer.Flush();
                }
                else
                {
                    Debug.Log("Disconnected non");
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Flush();
                    StreamReader reader = new StreamReader(stream);
                    msg = reader.ReadLine();
                    writer.WriteLine("Disconnect\r");
                    writer.Flush();
                }
                client.Close();
            }
            if (statusChanged != null)
            {
                statusChanged("Connection closed");
            }
			connecting = false;
		} catch {

		}
        //not connecting to any server
        // wait fpr listening thread to terminate (max. 500ms)
//		if (rThread != null) {
//	        rThread.Join(500);
//		}
    }
	
	void Reads(object sender, DoWorkEventArgs e)
	{
		
		while(mRunning)
		{
			NetworkStream stream = client.GetStream();
			stream.ReadTimeout = 10000;

				if (stream.DataAvailable)
				{
//					Debug.Log("Prepare to read");
					try {
						StreamReader reader = new StreamReader(stream);
					
						msg = reader.ReadLine();
	
						if(!msg.Contains("Initiate"))
						{
							values = msg.Split(","[0]);
						
							for (int j = 0; j < channel * 2; j++) //times 2 for raw channels
							{	
								serverSignals[j] = float.Parse(values[j]);
							}
	
							if (IncomingDataFromSensor != null) {
								IncomingDataFromSensor(serverSignals);
							}
						} else {					
							channel = int.Parse(msg.Split(':')[1]);
						}
	//					Debug.Log(msg);
						
						StreamWriter writer = new StreamWriter(stream);
						// \r is necessary for CocoaAsyncSocket to read
						writer.WriteLine("Acknowledged\r");
						writer.Flush();
	//					writer.AutoFlush = true;
	//					Debug.Log("WriteBack");			
					}
					catch {
						Debug.Log("can't read from server");
						mRunning = false;
					}	
				}

			}

		Debug.Log("break from while");
	}

    void OnDestroy()
    {
        stopListening();
    }

	void OnApplicationQuit()
	{
		stopListening();
	}
}