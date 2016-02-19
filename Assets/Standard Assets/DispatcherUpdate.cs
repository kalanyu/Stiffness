using UnityEngine;
using System;

public class DispatcherUpdate : MonoBehaviour
{
	void Update ()
	{
		Dispatcher.Instance.InvokePending();
	}
}

