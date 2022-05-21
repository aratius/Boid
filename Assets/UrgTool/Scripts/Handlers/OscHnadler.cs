using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityOSC;
using UnityEngine.Events;

public class OscHnadler : Singleton<OscHnadler> {
	private OSCReciever reciever;

	int port = 3333;
	
	public class ReciverEvent : UnityEvent<string, List<object>>
	{

	}

	public ReciverEvent reciverEvent = new ReciverEvent();


	public void Start(int port)
	{
		this.port = port;
		reciever = new OSCReciever();
		Debug.Log("INFO: OSC Reciver Open");
		reciever.Open(port);
	}

	public void OnDestroy() {
		Debug.Log("INFO: OSC Reciver Close");
		reciever.Close();
	}

	// Update is called once per frame
	void Update()
	{
		if(reciever.hasWaitingMessages()){
			OSCMessage msg = reciever.getNextMessage();
			Debug.Log(string.Format("message received: {0} {1}", msg.Address, DataToString(msg.Data)));
			reciverEvent.Invoke(msg.Address, msg.Data);
		}
	}

	private string DataToString(List<object> data)
	{
		string buffer = "";
		
		for(int i = 0; i < data.Count; i++)
		{
			buffer += data[i].ToString() + " ";
		}
		
		buffer += "\n";
		
		return buffer;
	}

	/***
	 * Send
	 ***/
	public void Send<T>(string ip, string address, T value)
	{
		Debug.Log("INFO: OSC Send: ip = " + ip + ", address = " + address);
		OSCHandler.Instance.CreateClient(ip, IPAddress.Parse(ip), port);
		OSCHandler.Instance.SendMessageToClient(ip, address, value);
	}

	public void Send<T>(string ip, string address, List<T> values)
	{
		Debug.Log("INFO: OSC Send: ip = " + ip + ", address = " + address);
		OSCHandler.Instance.CreateClient(ip, IPAddress.Parse(ip), port);
		OSCHandler.Instance.SendMessageToClient(ip, address, values);
	}
}
