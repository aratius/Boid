using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URG;

namespace Config {
	public class Settings {
		public static string jsonPath = Application.dataPath + "/setting.json";
		public static Vector2 urgRect;
		public static float scale;
		public static int touchDistance;
		public static float touchThoreshold;
		public static int minTouchSize;
		public static int maxTouchSize;
		public static TouchStartPosition touchStartPosition;
	}

	[System.Serializable]
	public class JsonData {
		public float scale;
		public int touchDistance;
		public float touchThoreshold;
		public int minTouchSize;
		public int maxTouchSize;
		public TouchStartPosition touchStartPosition;

		public JsonUrgData[] urgData; 
	}
	
	[System.Serializable]
	public class JsonUrgData {
		public string ip_address;
		public int port_number;
		public UrgStartPosition urgStartPosition;

		public Vector2 topLeft; //キャリブレーション 左上

		public Vector2 topRight; //キャリブレーション 右上

		public Vector2 bottomLeft; //キャリブレーション 左下

		public Vector2 bottomRight; //キャリブレーション 右下
		public Color distanceColor;
		public bool isDebug; //デバッグ表示確認

		public Color color;

	}
}
