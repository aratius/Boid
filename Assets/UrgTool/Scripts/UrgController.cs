using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrgController : MonoBehaviour
{
	[SerializeField]
	bool useOsc = true;

	[SerializeField]
	string oscIpToSend = "192.168.0.100"; //送信先IP

	[SerializeField]
	int port = 3333; //ポート

	UrgsHandler urgsHandler;
	
	OscHnadler oscHnadler;
	void Start() {
		urgsHandler = GetComponent<UrgsHandler>();
		oscHnadler = OscHnadler.Instance;
	}

	void OnDestroy() {
	}

	void Update() {
		//OSC送信
		if (useOsc && urgsHandler.sendPosition.Count > 0) {
			foreach(List<float> data in urgsHandler.sendPosition) {
				oscHnadler.Send(oscIpToSend, "/urg", data);
			}
		}
	}
}
