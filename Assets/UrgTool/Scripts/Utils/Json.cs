using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using URG;
using Config;

public class Json {
	public static void Save(JsonData data) {
		StreamWriter writer;
		string jsonstr = JsonUtility.ToJson(data);
		Debug.Log(jsonstr);

		writer = new StreamWriter(Settings.jsonPath, false);
		writer.Write(jsonstr);
		writer.Flush();
		writer.Close();
	}

	public static JsonData Load() {

		StreamReader reader;
		reader = new StreamReader(Settings.jsonPath);
		string jsonstr = reader.ReadToEnd();
		reader.Close();
 
		return JsonUtility.FromJson<JsonData> (jsonstr);

	}
}
